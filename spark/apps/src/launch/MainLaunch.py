from spark.apps.src.config.SparkSessionCustom import SparkSessionCustom
from spark.apps.src.launch.currencies.datamart.CurrenciesDatamart import CurrenciesDatamart
from spark.apps.src.launch.currencies.preprocess.CurrenciesRawDataPreprocess import CurrenciesRawDataPreprocess
from spark.apps.src.launch.currencies.analyze.CurrenciesFunctionalDataAnalyze import CurrenciesFunctionalDataAnalyze


class MainLaunch(SparkSessionCustom):

    def __init__(self):
        super().__init__()
        self.currencies_raw_data_preprocess = CurrenciesRawDataPreprocess(self.spark)
        self.currencies_functional_data_analyze = CurrenciesFunctionalDataAnalyze(self.spark)
        self.currencies_datamart = CurrenciesDatamart(self.spark)

    def start(self):
        self.currencies_raw_data_preprocess.start()
        self.currencies_functional_data_analyze.start()
        self.currencies_datamart.start()

        self.spark.streams.awaitAnyTermination()
