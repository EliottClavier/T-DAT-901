import os
from datetime import datetime
from influxdb import DataFrameClient
from pyspark.sql.functions import lit, col, udf

from spark.apps.src.config.SparkSessionCustom import SparkSessionCustom


class CurrenciesDatamart(SparkSessionCustom):

    def __init__(self):
        super().__init__()
        self.currencies_stream = self.read_currencies()
        self.client = DataFrameClient(
            host=os.environ['INFLUXDB_HOST'],
            port=os.environ['INFLUXDB_PORT'],
            username=os.environ["INFLUXDB_USER"],
            password=os.environ["INFLUXDB_PASSWORD"],
            database=os.environ["INFLUXDB_BUCKET"]
        )

    def start(self):
        self.currencies_stream.foreachBatch(self.transform)

    def read_currencies(self):
        return self.spark.readStream \
            .format("parquet") \
            .option("path", os.environ["PARQUET_PATH"]) \
            .option("checkpointLocation", os.environ["PARQUET_CHECKPOINT_LOCATION"]) \
            .load()

    def write_to_influx_db(self, output_df):
        measurement = "CryptoCurrency"

        self.client.write_points(output_df,
                            measurement)

    def transform(self, currencies_df):
        # Read exchanges df

        # Window function

        # Aggregate

        # Format dates
        convert_from_timestamp_udf = udf(lambda date: datetime.fromtimestamp(date))
        output_df = (currencies_df
                     .withColumn("dht", convert_from_timestamp_udf(col("dht")))
                     .withColumn("part_dhi", convert_from_timestamp_udf(col("part_dhi"))))

        # Write
        self.write_to_influx_db(output_df)




