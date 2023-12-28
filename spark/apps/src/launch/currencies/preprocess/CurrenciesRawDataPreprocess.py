import datetime
import os

from pyspark.sql.functions import lit, col
from spark.apps.src.config.SparkSessionCustom import SparkSessionCustom


class CurrenciesRawDataPreprocess(SparkSessionCustom):
    raw_stream = None
    dhi_timestamp = None

    def __init__(self):
        super().__init__()
        self.raw_stream = self.read_from_kafka()
        self.dhi_timestamp = datetime.datetime.now().timestamp()
        self.raw_stream.foreachPartition(self.transform(self.raw_stream, self.dhi_timestamp))

    def read_from_kafka(self):
        return self.spark.readStream \
            .format("kafka") \
            .option("kafka.bootstrap.servers", os.environ["KAFKA_BOOTSTRAP_SERVERS"]) \
            .option("subscribe", os.environ["KAFKA_DEFAULT_TOPIC"]) \
            .load()

    @staticmethod
    def write_to_parquet(df):
        df.write \
            .format("parquet") \
            .option("checkpointLocation", os.environ["PARQUET_CHECKPOINT_LOCATION"]) \
            .mode("append") \
            .save(os.environ["PARQUET_PATH"])

    @staticmethod
    def transform(input_df, dhi_timestamp):
        string_columns = [col(column).cast("string").alias(column) for column in input_df.columns]
        raw_stream_df = input_df.select(*string_columns)

        # Éliminer les lignes avec des valeurs nulles
        raw_stream_df = raw_stream_df.na.drop()

        # Add technical field
        raw_stream_df \
            .withColumn("dhi", lit(dhi_timestamp))

        CurrenciesRawDataPreprocess.write_to_parquet(raw_stream_df)
