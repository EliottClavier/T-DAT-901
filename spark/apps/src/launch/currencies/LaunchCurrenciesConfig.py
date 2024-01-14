base_path = "/data"
absolute_path = "/opt/bitnami/spark/spark/data"


class RawCurrenciesConfig:
    output_path = f"{base_path}/raw/currencies"
    absolute_output_path = f"{absolute_path}/raw/currencies"
    checkpoint_location = f"{absolute_output_path}/checkpoint"


class FunctionalCurrenciesConfig:
    input_path = RawCurrenciesConfig.output_path
    relative_input_path = RawCurrenciesConfig.absolute_output_path
    output_path = f"{base_path}/functional/currencies"
    absolute_output_path = f"{absolute_path}/functional/currencies"
    checkpoint_location = f"{absolute_output_path}/checkpoint"


class DatamartCurrenciesConfig:
    input_path = FunctionalCurrenciesConfig.output_path
    relative_input_path = FunctionalCurrenciesConfig.absolute_output_path
