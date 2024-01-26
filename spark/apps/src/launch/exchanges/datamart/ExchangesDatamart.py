from datetime import datetime
from influxdb_client import InfluxDBClient, Point
from influxdb_client.client.write_api import SYNCHRONOUS
from pyspark.sql.functions import col, sum
from pyspark.sql import DataFrame
from pyspark.sql.window import Window

from spark.apps.src.install.exchanges.schema.datamart.schema import datamart_schema
from spark.apps.src.install.exchanges.schema.functional.schema import functional_schema
from spark.apps.src.launch.exchanges.LaunchExchangesConfig import DatamartExchangesConfig as config, \
    DatamartExchangesConfig


class ExchangesDatamart:

    def __init__(self, spark):
        self.spark = spark
        self.exchanges_stream = self.read_exchanges_stream()
        self.client = InfluxDBClient.from_env_properties()
        self.write_api = self.client.write_api(write_options=SYNCHRONOUS)

    def start(self):
        self.exchanges_stream.writeStream \
            .outputMode("append") \
            .foreachBatch(self.transform) \
            .start()

    def read_exchanges_stream(self):
        return self.spark.readStream \
            .schema(functional_schema) \
            .option("cleanSource", "delete") \
            .parquet(config.absolute_input_tmp_path)

    def write_to_influx_db(self, output_df):
        pandas_df = output_df.toPandas()

        # Write each row as a point to InfluxDB
        for index, row in pandas_df.iterrows():
            timestamp = datetime.fromtimestamp(round(row['dhi']/1000))
            point = Point("CryptoExchange") \
                .tag("CurrencySymbol", row['CurrencySymbol']) \
                .field("TradeId", row['TradeId']) \
                .field("ExchangeName", row['ExchangeName']) \
                .field("MinuteAggregate", row['MinuteAggregate']) \
                .field("Price", row['Price']) \
                .field("Quantity", row['Quantity']) \
                .field("dht", row['dht']) \
                .time(timestamp)
            self.write_api.write(bucket="crypto_db", record=point)

    def read_exchanges_like_dhi(self, dhi_path: str, currency_symbol: str) -> DataFrame:
        return self.spark.read \
            .option("basePath", DatamartExchangesConfig.absolute_input_path) \
            .option("mergeSchema", "true") \
            .parquet(f"{DatamartExchangesConfig.absolute_input_path}/dhi={dhi_path}*") \
            .filter(col("CurrencySymbol") == currency_symbol)

    def calculate_quantity(self, exchanges_df: DataFrame, column_type: str) -> DataFrame:
        window_spec = Window.partitionBy("CurrencySymbol").orderBy("dhi")

        computed_df = exchanges_df \
            .withColumn(column_type, sum(col("Quantity")).over(window_spec)) \

        return computed_df

    def aggregate_exchanges(self, exchanges_df):
        aggregate_exchanges_df = self.spark.createDataFrame([], datamart_schema)
        for row in exchanges_df.distinct().collect():
            current_dhi = round(row['dhi']/1000)
            current_currency_symbol = row['CurrencySymbol']
            minute_dhi_path = datetime.fromtimestamp(current_dhi).strftime('%Y%m%d%H%M')

            dhi_exchanges_df = self.read_exchanges_like_dhi(minute_dhi_path, current_currency_symbol)

            exchange_aggregated_df = self.calculate_quantity(dhi_exchanges_df, "MinuteAggregate").first()

            aggregate_exchanges_result_row = self.spark.createDataFrame([(
                row['TradeId'],
                row['Price'],
                row['Quantity'],
                exchange_aggregated_df['MinuteAggregate'],
                current_currency_symbol,
                row['dhi'],
                row['ExchangeName'],
                row['dht'],
            )], datamart_schema)

            aggregate_exchanges_df = aggregate_exchanges_df.union(aggregate_exchanges_result_row)
        return aggregate_exchanges_df

    def transform(self, exchanges_df, epoch_id):
        # Aggregate
        exchanges_aggregated_df = self.aggregate_exchanges(exchanges_df)

        # Write
        self.write_to_influx_db(exchanges_aggregated_df)
