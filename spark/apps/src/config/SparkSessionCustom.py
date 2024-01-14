from pyspark.sql import SparkSession
import os
import sys

from spark.apps.src.launch.currencies.LaunchCurrenciesConfig import absolute_path

os.environ['PYSPARK_PYTHON'] = sys.executable
os.environ['PYSPARK_DRIVER_PYTHON'] = sys.executable


class SparkSessionCustom:
    spark: SparkSession = None

    def __init__(self):
        # Initialize Spark
        self.spark = SparkSession.builder \
            .appName("KafkaPySparkStreaming") \
            .config("spark.master", "local") \
            .config("spark.local.dir", absolute_path) \
            .getOrCreate()
