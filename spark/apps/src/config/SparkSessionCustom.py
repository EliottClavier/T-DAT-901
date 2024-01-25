from pyspark.sql import SparkSession
import os
import sys

os.environ['PYSPARK_PYTHON'] = sys.executable
os.environ['PYSPARK_DRIVER_PYTHON'] = sys.executable


class SparkSessionCustom:
    def __init__(self):
        self.spark = SparkSession \
            .builder \
            .appName("CryptoPySparkApplication") \
            .master("local[*]") \
            .getOrCreate()
