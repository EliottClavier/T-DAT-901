from unittest import TestCase
from pyspark.sql import SparkSession


class DefaultTestCase(TestCase):
    spark = None

    @classmethod
    def setUpClass(cls):
        super().setUpClass()
        cls.spark = SparkSession.builder.appName("Crypto PySpark App").getOrCreate()

    @classmethod
    def tearDownClass(cls):
        cls.spark.stop()
