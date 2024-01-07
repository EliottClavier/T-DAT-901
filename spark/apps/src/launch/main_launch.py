from spark.apps.src.launch.currencies.preprocess.CurrenciesRawDataPreprocess import CurrenciesRawDataPreprocess
from spark.apps.src.launch.currencies.analyze.CurrenciesFunctionalDataAnalyze import CurrenciesFunctionalDataAnalyze


def main():
    data_preprocessing_transformer = CurrenciesRawDataPreprocess()
    data_analyze_transformer = CurrenciesFunctionalDataAnalyze()

    data_preprocessing_transformer.transform()
    data_analyze_transformer.transform()


if __name__ == "__main__":
    main()
