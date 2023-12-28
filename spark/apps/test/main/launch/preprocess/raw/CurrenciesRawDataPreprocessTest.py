import datetime
import os
from unittest import mock

from pyspark.testing.utils import assertDataFrameEqual
from spark.apps.src.launch.currencies.preprocess.CurrenciesRawDataPreprocess import CurrenciesRawDataPreprocess

from spark.apps.src.config.PySparkTestCase import PySparkTestCase


class CurrenciesRawDataPreprocessTest(PySparkTestCase):

    def test_transform(self):
        test_list = [
            ("tu1", "Nominal case"),
            ("tu2", "Input already exists"),
            ("tu3", "Multliples lines to add"),
        ]

        for name, title in test_list:
            test_root_path = f'../../../../ressources/preprocess/currencies/raw/{name}'

            input_df = self.spark.read.format("json").load(f'{test_root_path}/currencies.json')

            mock.patch.dict(os.environ, {"PARQUET_PATH": f'{test_root_path}/inputRaw'}).start()
            CurrenciesRawDataPreprocess.transform(input_df, datetime.datetime.now().timestamp())

            actual = self.spark.read.parquet(f'{test_root_path}/inputRaw')

            expected = self.spark.read.parquet(f'{test_root_path}/expected')

            assertDataFrameEqual(actual, expected)
            mock.patch.dict(os.environ, {"PARQUET_PATH": ""}).stop()
            PySparkTestCase.clean_input_directory(f'{test_root_path}/inputRaw')
