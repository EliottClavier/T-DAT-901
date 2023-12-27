import unittest
from unittest import mock
from pyspark.sql import SparkSession


class PySparkTestCase(unittest.TestCase):

    mock_cfg = None

    @classmethod
    def setUpClass(cls):
        cls.spark = SparkSession.builder.appName("Testing PySpark Example").getOrCreate()
        cls.mock_cfg = mock.patch.dict('os.environ', {
            "KAFKA_BOOTSTRAP_SERVERS": "KAFKA_BOOTSTRAP_SERVERS",
            "KAFKA_DEFAULT_TOPIC": "KAFKA_DEFAULT_TOPIC",
            "PARQUET_PATH": "./parquet",
            "PARQUET_CHECKPOINT_LOCATION": "./parquet"
        })
        cls.mock_cfg.start()

    @classmethod
    def tearDownClass(cls):
        cls.spark.stop()
        cls.mock_cfg.stop()
