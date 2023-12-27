from spark.apps.src.launch.currencies.preprocess.CurrenciesRawDataPreprocess import CurrenciesRawDataPreprocess
from spark.apps.src.launch.currencies.analyze.CurrenciesFunctionalDataAnalyze import CurrenciesFunctionalDataAnalyze


def main():
    dataPreprocessingTransformer = CurrenciesRawDataPreprocess()
    dataAnalyzeTransformer = CurrenciesFunctionalDataAnalyze()

    dataPreprocessingTransformer.transform()
    dataAnalyzeTransformer.transform()


if __name__ == "__main__":
    main()
