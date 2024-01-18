import os
from pyspark.sql.functions import col, from_json, lit

from spark.apps.src.launch.common.utils import get_dht, parse_kafka_df
from spark.apps.src.launch.currencies.LaunchCurrenciesConfig import RawCurrenciesConfig as config
from spark.apps.src.install.currencies.schema.input.schema import input_schema


class CurrenciesRawDataPreprocess:
    def __init__(self, spark):
        self.spark = spark
        self.raw_stream = self.read_from_kafka()

    def start(self):
        self.raw_stream.writeStream \
            .foreachBatch(self.transform) \
            .start()

    def read_from_kafka(self):
        return self.spark.readStream \
            .format("kafka") \
            .option("kafka.bootstrap.servers", os.environ["KAFKA_BOOTSTRAP_SERVERS"]) \
            .option("subscribe", os.environ["KAFKA_DEFAULT_TOPIC"]) \
            .load()

    def transform(self, input_df, epoch_id):
        dht = get_dht()

        parsed_df = parse_kafka_df(input_df, input_schema)

        parsed_df = parsed_df.withColumn("part_dht", lit(str(dht)))

        parsed_df.write \
            .mode("append") \
            .json(config.absolute_output_path)
