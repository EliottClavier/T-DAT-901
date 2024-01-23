import json
from datetime import datetime
from pyspark.sql import SparkSession
from pyspark.sql.window import Window
from pyspark.sql import DataFrame, Row
from pyspark.sql.functions import col, min, max, first, last, hour, from_unixtime

from spark.apps.src.install.currencies.schema.datamart.schema import candle_schema, candle_hour_schema
from spark.apps.src.launch.currencies.LaunchCurrenciesConfig import DatamartCurrenciesConfig


class CandleBuilder:

    def __init__(self, spark: SparkSession):
        self.spark = spark

    def read_currencies_like_dhi(self, dhi_path: str, currency_name: str) -> DataFrame:
        return self.spark.read \
            .option("basePath", DatamartCurrenciesConfig.absolute_input_path) \
            .option("mergeSchema", "true") \
            .parquet(f"{DatamartCurrenciesConfig.absolute_input_path}/dhi={dhi_path}*") \
            .filter(col("CurrencyName") == currency_name)

    def fill_candle(self, currencies_df: DataFrame, candle_column_name: str) -> DataFrame:
        windowSpec = Window.partitionBy("CurrencyName").orderBy("dhi")

        computed_df = currencies_df \
            .withColumn(f"{candle_column_name}.lowestPrice", min(col("Price")).over(windowSpec)) \
            .withColumn(f"{candle_column_name}.highestPrice", max(col("Price")).over(windowSpec)) \
            .withColumn(f"{candle_column_name}.openingPrice", last(col("Price")).over(windowSpec)) \
            .withColumn(f"{candle_column_name}.closurePrice", first(col("Price")).over(windowSpec))

        return computed_df

    def build_candle(self,
                     currencies_row: Row,
                     column_name: str,
                     dhi_path: str) -> str:
        current_dhi = currencies_row['dhi']
        current_dht = currencies_row['dht']
        minute_dhi_path = datetime.fromtimestamp(current_dhi).strftime(dhi_path)
        currencies_df = self.read_currencies_like_dhi(minute_dhi_path, currencies_row['CurrencyName'])
        filled_candle_df = self.fill_candle(currencies_df, column_name)

        candle = filled_candle_df \
            .filter(col("dhi") == current_dhi) \
            .filter(col("dht") == current_dht) \
            .first()[column_name]

        return json.dumps(candle)

    def build_candles(self, currencies_df: DataFrame):
        candles_result_df = self.spark.createDataFrame([], candle_schema)

        for row in currencies_df.distinct().collect():
            current_dhi = row['dhi']
            current_currency_name = row['CurrencyName']
            minute_dhi_path = datetime.fromtimestamp(current_dhi).strftime('%Y%m%d%H%M')

            dhi_currencies_df = self.read_currencies_like_dhi(minute_dhi_path, current_currency_name)

            filled_candle_df = self.fill_candle(dhi_currencies_df, "minuteCandle").first()

            candles_result_row = self.spark.createDataFrame([(
                current_currency_name,
                current_dhi,
                row['dht'],
                json.dumps({
                    "lowestPrice": filled_candle_df["minuteCandle.lowestPrice"],
                    "highestPrice": filled_candle_df["minuteCandle.highestPrice"],
                    "openingPrice": filled_candle_df["minuteCandle.openingPrice"],
                    "closurePrice": filled_candle_df["minuteCandle.closurePrice"]
                })
            )], candle_schema)

            candles_result_df = candles_result_df.union(candles_result_row)

        return candles_result_df

    def build_hourly_candles(self, currencies_df):
        candles_result_df = self.spark.createDataFrame([], candle_hour_schema)

        for row in currencies_df.distinct().collect():
            current_dhi = row['dhi']
            current_currency_name = row['CurrencyName']
            hourly_dhi_path = datetime.fromtimestamp(current_dhi).strftime('%Y%m%d%H')

            dhi_currencies_df = self.read_currencies_like_dhi(hourly_dhi_path, current_currency_name)

            filled_candle_df = self.fill_candle(dhi_currencies_df, "hourCandle").first()

            candles_result_row = self.spark.createDataFrame([(
                current_currency_name,
                current_dhi,
                row['dht'],
                json.dumps({
                    "lowestPrice": filled_candle_df["hourCandle.lowestPrice"],
                    "highestPrice": filled_candle_df["hourCandle.highestPrice"],
                    "openingPrice": filled_candle_df["hourCandle.openingPrice"],
                    "closurePrice": filled_candle_df["hourCandle.closurePrice"]
                })
            )], candle_hour_schema)

            candles_result_df = candles_result_df.union(candles_result_row)

        return candles_result_df
