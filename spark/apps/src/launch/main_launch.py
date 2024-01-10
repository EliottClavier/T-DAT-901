from spark.apps.src.launch.currencies.preprocess.CurrenciesRawDataPreprocess import CurrenciesRawDataPreprocess
from spark.apps.src.launch.currencies.analyze.CurrenciesFunctionalDataAnalyze import CurrenciesFunctionalDataAnalyze


def main():
    # Launch currencies flux transformers
    CurrenciesRawDataPreprocess().start()
    CurrenciesFunctionalDataAnalyze().start()



if __name__ == "__main__":
    main()
