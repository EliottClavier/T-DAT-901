import datetime
import pyspark.testing
from pyspark.testing.utils import assertDataFrameEqual
from spark.apps.src.config.PySparkTestCase import PySparkTestCase
from spark.apps.src.launch.currencies.preprocess.CurrenciesRawDataPreprocess import CurrenciesRawDataPreprocess


class CurrenciesRawDataPreprocessTest(PySparkTestCase):

    def test_transform(self):
        testList = [
            {"tu1", "Nominal case"},
        ]

        for name, title in testList:
            testRootPath = f'../../../ressources/preprocess/currencies/raw/{name}'

            input_df = self.spark.read.format("json").load(f'{testRootPath}/currencies.json')

            actual = CurrenciesRawDataPreprocess.transform(input_df, datetime.datetime.now().timestamp())

            expected = self.spark.read.format("json").load(f'{testRootPath}/expected.json')

            assertDataFrameEqual(actual, expected)
