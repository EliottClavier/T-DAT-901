import os
from unittest import mock
from pyspark.testing.utils import assertDataFrameEqual

from spark.apps.src.launch.currencies.preprocess.CurrenciesRawDataPreprocess import CurrenciesRawDataPreprocess
from spark.apps.src.install.currencies.schema.raw.schema import raw_schema
from spark.apps.src.install.currencies.schema.input.schema import input_schema
from spark.apps.test.config.DefaultTestCase import DefaultTestCase
from spark.apps.test.resources.preprocess.currencies.raw.config import root_path, relative_root_path, test_dht
from spark.apps.test.config.transform.TransformTest import test_transform


class CurrenciesRawDataPreprocessTest(DefaultTestCase):

    @classmethod
    def setUpClass(cls):
        super().setUpClass()
        cls.tests_list = [
            ("tu1", "Nominal case"),
            ("tu2", "Input already exists"),
            ("tu3", "Multliples lines to add"),
        ]
        cls.test_files_path_names = ["expected", "inputRaw"]
        cls.input_format = "parquet"
        cls.root_path = root_path

    @test_transform
    def test_transform(self):
        for name, title in self.tests_list:
            with self.subTest(name=name, title=title):
                test_root_path = f'{root_path}/{name}'
                relative_parquet_path = f'{relative_root_path}/{name}/parquet'
                parquet_path = f'{test_root_path}/parquet'

                input_df = (self.spark
                            .readStream
                            .option("inferSchema", "true")
                            .format("parquet")
                            .schema(input_schema)
                            .load(f'{test_root_path}/currencies*.parquet'))

                mock.patch.dict(os.environ, {"PARQUET_PATH": f'{relative_parquet_path}',
                                             "PARQUET_CHECKPOINT_LOCATION": f'{relative_parquet_path}/checkpoint',
                                             "DHT": test_dht}).start()

                CurrenciesRawDataPreprocess.transform(input_df).awaitTermination(1)

                expected = (self.spark
                            .read
                            .schema(raw_schema)
                            .option("mergeSchema", "true")
                            .parquet(f'{test_root_path}/expected'))

                actual = (self.spark
                          .read
                          .schema(raw_schema)
                          .option("mergeSchema", "true")
                          .parquet(f'{parquet_path}/part*.parquet'))

                assertDataFrameEqual(actual, expected)
                mock.patch.dict(os.environ, {"PARQUET_PATH": ""}).stop()
