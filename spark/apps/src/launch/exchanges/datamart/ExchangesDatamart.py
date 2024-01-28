from datetime import datetime
from influxdb_client import InfluxDBClient, Point
from influxdb_client.client.write_api import SYNCHRONOUS
from pyspark.sql.functions import col, sum, date_format, from_unixtime
from pyspark.sql import DataFrame
from pyspark.sql.window import Window

from spark.apps.src.install.exchanges.schema.functional.schema import functional_schema
from spark.apps.src.launch.exchanges.LaunchExchangesConfig import DatamartExchangesConfig as config, \
    DatamartExchangesConfig


class ExchangesDatamart:

    def __init__(self, spark):
        self.spark = spark
        self.exchanges_stream, self.volume_stream = self.read_exchanges_stream()
        self.client = InfluxDBClient.from_env_properties()
        self.write_api = self.client.write_api(write_options=SYNCHRONOUS)

    def start(self):
        self.exchanges_stream.writeStream \
            .outputMode("append") \
            .foreachBatch(self.transform) \
            .start()

        self.volume_stream.writeStream \
            .outputMode("append") \
            .trigger(processingTime='30 seconds') \
            .foreachBatch(self.aggregate_exchanges) \
            .start()

    def read_exchanges_stream(self):
        trade_main_stream = self.spark.readStream \
            .schema(functional_schema) \
            .parquet(config.absolute_input_tmp_path)

        trade_volume_stream = self.spark.readStream \
            .schema(functional_schema) \
            .option("cleanSource", "delete") \
            .parquet(config.absolute_input_tmp_path)

        return trade_main_stream, trade_volume_stream

    def calculate_quantity(self, exchanges_df: DataFrame, column_type: str) -> DataFrame:
        window_spec = Window.partitionBy("CurrencySymbol").orderBy("dhi")

        computed_df = exchanges_df \
            .withColumn(column_type, sum(col("Quantity")).over(window_spec))

        return computed_df

    def aggregate_exchanges(self, exchanges_df, epoch_id):
        exchanges_df = exchanges_df.withColumn("minute_dhi_path", date_format(from_unixtime("dhi"), 'yyyyMMddHHmm'))

        pandas_df = exchanges_df.toPandas()

        actual_dhi_computed = []

        for index, row in pandas_df.iterrows():
            if row['dhi'] not in actual_dhi_computed:
                actual_dhi_computed.append(row['dhi'])
                history_df = self.read_exchanges_like_dhi(row['minute_dhi_path'], row['CurrencySymbol'])

                point = Point("CryptoTradesVolume") \
                    .tag("CurrencySymbol", row['CurrencySymbol']) \
                    .field("VolumeMinute", history_df.agg({"Quantity": "sum"}).collect()[0][0]) \
                    .field("dht", row['dht']) \
                    .time(datetime.fromtimestamp(row['dhi']))
                self.write_api.write(bucket="crypto_db", record=point)

    def read_exchanges_like_dhi(self, dhi_path: str, currency_symbol: str) -> DataFrame:
        return self.spark.read \
            .option("basePath", DatamartExchangesConfig.absolute_input_path) \
            .option("mergeSchema", "true") \
            .parquet(f"{DatamartExchangesConfig.absolute_input_path}/CurrencySymbol={currency_symbol}/dhi={dhi_path}*")

    def transform(self, exchanges_df, epoch_id):
        for index, row in exchanges_df.toPandas().iterrows():
            point = Point("CryptoExchange") \
                .tag("CurrencySymbol", row['CurrencySymbol']) \
                .field("TradeId", row['TradeId']) \
                .field("Price", row['Price']) \
                .field("Quantity", row['Quantity']) \
                .field("dht", row['dht']) \
                .time(datetime.fromtimestamp(row['dhi']))
            self.write_api.write(bucket="crypto_db", record=point)
