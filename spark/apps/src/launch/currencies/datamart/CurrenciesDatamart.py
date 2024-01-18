from datetime import datetime
import os
import pandas
from influxdb_client import InfluxDBClient, Point
from influxdb_client.client.write_api import SYNCHRONOUS

from spark.apps.src.launch.currencies.LaunchCurrenciesConfig import DatamartCurrenciesConfig as config
from spark.apps.src.install.currencies.schema.functional.schema import functional_schema


class CurrenciesDatamart:

    def __init__(self, spark):
        self.spark = spark
        self.currencies_stream = self.read_currencies()
        self.client = InfluxDBClient.from_env_properties()
        self.write_api = self.client.write_api(write_options=SYNCHRONOUS)

    def start(self):
        self.currencies_stream.writeStream \
            .outputMode("append") \
            .foreachBatch(self.transform) \
            .start()

    def read_currencies(self):
        return self.spark.readStream \
            .schema(functional_schema) \
            .json(config.absolute_input_path)

    def write_to_influx_db(self, output_df):
        pandas_df = output_df.toPandas()

        # Write each row as a point to InfluxDB
        for index, row in pandas_df.iterrows():
            point = Point("CryptoCurrency") \
                .tag("CurrencyName", row['CurrencyName']) \
                .field("ExchangeName", row['ExchangeName']) \
                .field("Price", row['Price']) \
                .field("dht", row['dht']) \
                .time(datetime.fromtimestamp(row['part_dhi']))
            self.write_api.write(bucket="crypto_db", record=point)

    def transform(self, currencies_df, epoch_id):
        # Read exchanges df

        # Window function

        # Aggregate

        # Write
        self.write_to_influx_db(currencies_df)
