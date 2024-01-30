import os
from spark.apps.src.launch.exchanges.LaunchExchangesConfig import RawExchangesConfig as config
from pyspark.sql.functions import lit, udf, col
from spark.apps.src.config.SparkSessionCustom import SparkSessionCustom
from spark.apps.src.install.exchanges.schema.input.schema import input_schema
from spark.apps.src.launch.common.utils import parse_kafka_df, get_dht


class ExchangesRawDataPreprocess(SparkSessionCustom):
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
            .option("subscribe", os.environ["KAFKA_TRANSACTION_TOPIC"]) \
            .load()

    def transform(self, input_df, epoch_id):
        dht = get_dht()

        parsed_df = parse_kafka_df(input_df, input_schema)

        parsed_df = parsed_df.na.drop()

        fix_timestamp = udf(lambda x: str(round(float(x) / 1000)), input_schema["TimeStamp"].dataType)

        parsed_df = parsed_df.withColumn("part_dht", lit(str(dht)))

        parsed_df = parsed_df.withColumn("TimeStamp", fix_timestamp(col("TimeStamp")))

        parsed_df.write \
            .mode("append") \
            .parquet(config.absolute_output_path)
