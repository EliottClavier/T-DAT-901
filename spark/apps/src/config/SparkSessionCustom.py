from pyspark.sql import SparkSession


class SparkSessionCustom:
    spark: SparkSession = None

    def __init__(self):
        # Initialize Spark
        self.spark = SparkSession.builder \
            .appName("KafkaPySparkStreaming") \
            .getOrCreate()
