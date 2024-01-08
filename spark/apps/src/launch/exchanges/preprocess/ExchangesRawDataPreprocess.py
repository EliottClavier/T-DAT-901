import datetime
import os

from pyspark.sql.functions import lit, col
from spark.apps.src.config.SparkSessionCustom import SparkSessionCustom


class ExchangesRawDataPreprocess(SparkSessionCustom):
    raw_stream = None

    def __init__(self):
        super().__init__()
        self.raw_stream = self.read_from_kafka()

    def start(self):
        self.raw_stream.foreachBatch(self.transform)

    def read_from_kafka(self):
        return self.spark.readStream \
            .format("kafka") \
            .option("kafka.bootstrap.servers", os.environ["KAFKA_BOOTSTRAP_SERVERS"]) \
            .option("subscribe", os.environ["KAFKA_DEFAULT_TOPIC"]) \
            .load()

    @staticmethod
    def transform(input_df):
        dht = os.environ["DHT"] if os.environ["DHT"] is not None else datetime.now().timestamp()

        # Add technical field
        input_df = input_df \
            .withColumn("dht", lit(dht))

        string_columns = [col(column).cast("string").alias(column) for column in input_df.columns]
        raw_stream_df = input_df.select(*string_columns)

        # Éliminer les lignes avec des valeurs nulles
        raw_stream_df = raw_stream_df.na.drop()

        return (raw_stream_df.writeStream
                .outputMode("append")
                .format("parquet")
                .option("checkpointLocation", os.environ["PARQUET_CHECKPOINT_LOCATION"])
                .start(os.environ["PARQUET_PATH"]))
