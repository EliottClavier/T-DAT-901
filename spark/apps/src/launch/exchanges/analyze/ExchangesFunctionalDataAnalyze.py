from pyspark.sql.functions import col, from_unixtime, date_format
from datetime import datetime
from influxdb_client import InfluxDBClient, Point
from influxdb_client.client.write_api import SYNCHRONOUS
from spark.apps.src.config.SparkSessionCustom import SparkSessionCustom
from spark.apps.src.install.exchanges.schema.functional.schema import functional_schema
from spark.apps.src.install.exchanges.schema.raw.schema import raw_schema
from spark.apps.src.launch.exchanges.LaunchExchangesConfig import FunctionalExchangesConfig as config


class ExchangesFunctionalDataAnalyze(SparkSessionCustom):
    def __init__(self, spark):
        self.spark = spark
        self.functional_stream = self.read_from_parquet()
        self.client = InfluxDBClient.from_env_properties()
        self.write_api = self.client.write_api(write_options=SYNCHRONOUS)

    def start(self):
        self.functional_stream.writeStream \
            .foreachBatch(self.transform) \
            .start()

    def read_from_parquet(self):
        return self.spark.readStream \
            .schema(raw_schema) \
            .option("cleanSource", "delete") \
            .parquet(config.absolute_input_path)

    def write_to_influx_db(self, output_df):
        pandas_df = output_df.toPandas()

        for index, row in pandas_df.iterrows():
            point = Point("CryptoExchanges") \
                .tag("CurrencySymbol", row['CurrencySymbol']) \
                .field("TradeId", row['TradeId']) \
                .field("Price", row['Price']) \
                .field("Quantity", row['Quantity']) \
                .field("dht", row['dht']) \
                .time(datetime.fromtimestamp(row['dhi']))
            self.write_api.write(bucket="crypto_db", record=point)

    def transform(self, input_df, epoch_id):
        input_df = (input_df
                    .withColumnRenamed("part_dht", "dht")
                    .withColumnRenamed("TimeStamp", "dhi"))

        output_df = input_df.select([col(field.name)
                                    .cast(field.dataType)
                                    .alias(field.name) for field in functional_schema.fields])

        self.write_to_influx_db(output_df)
