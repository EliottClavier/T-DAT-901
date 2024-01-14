from spark.apps.src.launch.currencies.datamart.CurrenciesDatamart import CurrenciesDatamart
from spark.apps.src.launch.currencies.preprocess.CurrenciesRawDataPreprocess import CurrenciesRawDataPreprocess
from spark.apps.src.launch.currencies.analyze.CurrenciesFunctionalDataAnalyze import CurrenciesFunctionalDataAnalyze


class MainLaunch:

    def __init__(self):
        self.currencies_raw_data_preprocess = CurrenciesRawDataPreprocess()
        self.currencies_functional_data_analyze = CurrenciesFunctionalDataAnalyze()
        self.currencies_datamart = CurrenciesDatamart()

    def start(self):
        self.currencies_raw_data_preprocess.start()
        self.currencies_functional_data_analyze.start()
        self.currencies_datamart.start()
