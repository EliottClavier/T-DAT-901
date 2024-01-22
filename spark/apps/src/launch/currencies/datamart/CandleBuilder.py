import json
from datetime import datetime
from pyspark.sql import SparkSession
from pyspark.sql.types import StringType
from pyspark.sql.window import Window
from pyspark.sql import DataFrame, Row
from pyspark.sql.functions import col, min, max, first, last, udf

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
            .withColumn(f"{candle_column_name}.openingPrice", first(col("Price")).over(windowSpec)) \
            .withColumn(f"{candle_column_name}.closurePrice", last(col("Price")).over(windowSpec))

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
        column_and_filepath_candles_list = [("minuteCandle", '%Y%m%d%H%M')]

        # Définir la spécification de la fenêtre
        windowSpec = Window.partitionBy("CurrencyName").orderBy("dhi")

        # Calculer les valeurs des bougies directement sur le DataFrame
        computed_df = currencies_df \
            .withColumn("lowestPrice", min(col("Price")).over(windowSpec)) \
            .withColumn("highestPrice", max(col("Price")).over(windowSpec)) \
            .withColumn("openingPrice", first(col("Price")).over(windowSpec)) \
            .withColumn("closurePrice", last(col("Price")).over(windowSpec))

        # Créer une représentation JSON des bougies
        to_json_udf = udf(lambda lowestPrice, highestPrice, openingPrice, closurePrice: json.dumps({
            "lowestPrice": lowestPrice,
            "highestPrice": highestPrice,
            "openingPrice": openingPrice,
            "closurePrice": closurePrice
        }), StringType())

        return computed_df.withColumn("minuteCandle", to_json_udf(
            col("lowestPrice"), col("highestPrice"), col("openingPrice"), col("closurePrice")))
