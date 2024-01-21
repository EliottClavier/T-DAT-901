flux_name = "currencies"
base_path = f"/data/{flux_name}"
absolute_path = f"/opt/bitnami/spark/spark/data/{flux_name}"


class RawCurrenciesConfig:
    output_path = f"{base_path}/raw"
    absolute_output_path = f"{absolute_path}/raw"
    checkpoint_location = f"{absolute_output_path}/checkpoint"


class FunctionalCurrenciesConfig:
    input_path = RawCurrenciesConfig.output_path
    absolute_input_path = RawCurrenciesConfig.absolute_output_path
    output_path = f"{base_path}/functional"
    absolute_output_path = f"{absolute_path}/functional"
    absolute_output_tmp_path = f"{absolute_output_path}/tmp"
    checkpoint_location = f"{absolute_output_path}/checkpoint"


class DatamartCurrenciesConfig:
    input_path = FunctionalCurrenciesConfig.output_path
    absolute_input_path = FunctionalCurrenciesConfig.absolute_output_path
