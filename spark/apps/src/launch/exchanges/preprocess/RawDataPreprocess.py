import datetime
import os

from pyspark.sql.functions import lit
from spark.apps.src.config.SparkSessionCustom import SparkSessionCustom


class RawDataPreprocess(SparkSessionCustom):
    raw_stream = None
    dhi_timestamp = None

    def __init__(self):
        super().__init__()
        self.raw_stream = self.read_from_kafka()
        self.dhi_timestamp = datetime.datetime.now().timestamp()

    def read_from_kafka(self):
        return self.spark.readStream \
            .format("kafka") \
            .option("kafka.bootstrap.servers", os.environ["KAFKA_BOOTSTRAP_SERVERS"]) \
            .option("subscribe", os.environ["KAFKA_DEFAULT_TOPIC"]) \
            .load()

    @staticmethod
    def write_to_parquet(df):
        df.writeStream \
            .format("parquet") \
            .option("path", os.environ["PARQUET_PATH"]) \
            .option("checkpointLocation", os.environ["PARQUET_CHECKPOINT_LOCATION"]) \
            .start() \
            .awaitTermination()

    def transform(self):
        raw_stream_df = self.raw_stream.selectExpr("CAST(value AS STRING) as value")

        # Clean data

        # Add technical field
        raw_stream_df \
            .withColumn("dhi", lit(self.dhi_timestamp))

        self.write_to_parquet(raw_stream_df)
