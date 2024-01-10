import os

from spark.apps.src.config.SparkSessionCustom import SparkSessionCustom
from spark.apps.src.install.currencies.functional.schema import functional_schema


class CurrenciesFunctionalDataAnalyze(SparkSessionCustom):
    functional_stream = None

    def __init__(self):
        super().__init__()
        self.functional_stream = self.read_from_parquet()

    def start(self):
        self.functional_stream.foreachBatch(self.transform)

    def read_from_parquet(self):
        return self.spark.readStream \
            .format("parquet") \
            .option("path", os.environ["PARQUET_PATH"]) \
            .option("checkpointLocation", os.environ["PARQUET_CHECKPOINT_LOCATION"]) \
            .load()

    def transform(self, input_df):
        input_df = (input_df
                    .withColumnRenamed("part_dht", "dht")
                    .withColumnRenamed("Timestamp", "part_dhi"))

        rdd = input_df.rdd
        output_df = self.spark.createDataFrame(rdd, schema=functional_schema)

        return (output_df.writeStream
                .outputMode("append")
                .format("parquet")
                .option("checkpointLocation", os.environ["PARQUET_CHECKPOINT_LOCATION"])
                .start(os.environ["PARQUET_PATH"]))
