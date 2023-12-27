from spark.apps.src.config.SparkSessionCustom import SparkSessionCustom
import os


class CurrenciesFunctionalDataAnalyze(SparkSessionCustom):
    functional_stream = None

    def __init__(self):
        super().__init__()
        self.raw_stream = self.read_from_parquet()

    def read_from_parquet(self):
        return self.spark.readStream \
            .format("parquet") \
            .option("path", os.environ["PARQUET_PATH"]) \
            .option("checkpointLocation", os.environ["PARQUET_CHECKPOINT_LOCATION"]) \
            .load()

    def transform(self):
        pass