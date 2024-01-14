import os
from spark.apps.src.launch.currencies.LaunchCurrenciesConfig import RawCurrenciesConfig, FunctionalCurrenciesConfig, \
    DatamartCurrenciesConfig, absolute_path


def give_permissions(path):
    os.chmod(path, 0o777)
    os.chown(path, 1001, 1001)


def build_dirs():
    os.makedirs(f"{absolute_path}/raw", exist_ok=True)
    os.makedirs(f"{absolute_path}/raw/currencies", exist_ok=True)
    os.makedirs(f"{absolute_path}/functional", exist_ok=True)
    os.makedirs(f"{absolute_path}/functional/currencies", exist_ok=True)


class MainInstall:

    paths = None

    def __init__(self):
        self.paths = [f"{absolute_path}/raw",
                      f"{absolute_path}/raw/currencies",
                      f"{absolute_path}/functional",
                      f"{absolute_path}/functional/currencies"]

    def start(self):
        # give_permissions(absolute_path)
        build_dirs()
        # for path in self.paths:
        # give_permissions(path)
