import os
from spark.apps.src.launch.currencies.LaunchCurrenciesConfig import RawCurrenciesConfig, FunctionalCurrenciesConfig, \
    absolute_path
from spark.apps.src.launch.exchanges.LaunchExchangesConfig import RawExchangesConfig, FunctionalExchangesConfig, \
    exchanges_absolute_path


def build_dirs():
    os.makedirs(absolute_path, exist_ok=True)
    os.makedirs(RawCurrenciesConfig.absolute_output_path, exist_ok=True)
    os.makedirs(FunctionalCurrenciesConfig.absolute_output_path, exist_ok=True)
    os.makedirs(FunctionalCurrenciesConfig.absolute_output_tmp_path, exist_ok=True)
    os.makedirs(exchanges_absolute_path, exist_ok=True)
    os.makedirs(RawExchangesConfig.absolute_output_path, exist_ok=True)
    os.makedirs(FunctionalExchangesConfig.absolute_output_path, exist_ok=True)
    os.makedirs(FunctionalExchangesConfig.absolute_output_tmp_path, exist_ok=True)


class MainInstall:
    def start(self):
        build_dirs()
