from datetime import datetime
from influxdb_client import InfluxDBClient, Point
from influxdb_client.client.write_api import SYNCHRONOUS

from spark.apps.src.launch.currencies.LaunchCurrenciesConfig import DatamartCurrenciesConfig as config
from spark.apps.src.install.currencies.schema.functional.schema import functional_schema
from spark.apps.src.launch.currencies.datamart.CandleBuilder import CandleBuilder


class CurrenciesDatamart:

    def __init__(self, spark):
        self.spark = spark
        self.currencies_stream = self.read_currencies_stream()
        self.client = InfluxDBClient.from_env_properties()
        self.write_api = self.client.write_api(write_options=SYNCHRONOUS)
        self.candle_builder = CandleBuilder(self.spark)

    def start(self):
        self.currencies_stream.writeStream \
            .outputMode("append") \
            .foreachBatch(self.transform) \
            .start()

    def read_currencies_stream(self):
        return self.spark.readStream \
            .schema(functional_schema) \
            .option("cleanSource", "delete") \
            .parquet(config.absolute_input_tmp_path)

    def write_candle_to_influx_db(self, candle_df):
        pandas_df = candle_df.toPandas()
        for index, row in pandas_df.iterrows():
            point = Point("CryptoCurrencyCandles") \
                .tag("CurrencyName", row['CurrencyName']) \
                .field("dht", row['dht']) \
                .field("minuteCandle", row['minuteCandle']) \
                .time(datetime.fromtimestamp(row['dhi']))
            self.write_api.write(bucket="crypto_db", record=point)

    def write_to_influx_db(self, output_df):
        pandas_df = output_df.toPandas()

        # Write each row as a point to InfluxDB
        for index, row in pandas_df.iterrows():
            point = Point("CryptoCurrency") \
                .tag("CurrencyName", row['CurrencyName']) \
                .field("ExchangeName", row['ExchangeName']) \
                .field("Price", row['Price']) \
                .field("dht", row['dht']) \
                .time(datetime.fromtimestamp(row['dhi']))
            self.write_api.write(bucket="crypto_db", record=point)

    def transform(self, currencies_df, epoch_id):
        # Aggregate
        self.write_candle_to_influx_db(self.candle_builder.build_candles(currencies_df))

        # Write
        self.write_to_influx_db(currencies_df)
