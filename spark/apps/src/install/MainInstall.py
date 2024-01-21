import os
from spark.apps.src.launch.currencies.LaunchCurrenciesConfig import RawCurrenciesConfig, FunctionalCurrenciesConfig, \
    absolute_path


def build_dirs():
    os.makedirs(absolute_path, exist_ok=True)
    os.makedirs(RawCurrenciesConfig.absolute_output_path, exist_ok=True)
    os.makedirs(FunctionalCurrenciesConfig.absolute_output_path, exist_ok=True)
    os.makedirs(FunctionalCurrenciesConfig.absolute_output_tmp_path, exist_ok=True)


class MainInstall:
    def start(self):
        build_dirs()
