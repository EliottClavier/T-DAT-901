flux_name = "exchanges"
base_path = f"/data/{flux_name}"
exchanges_absolute_path = f"/opt/bitnami/spark/spark/data/{flux_name}"


class RawExchangesConfig:
    output_path = f"{base_path}/raw"
    absolute_output_path = f"{exchanges_absolute_path}/raw"
    checkpoint_location = f"{absolute_output_path}/checkpoint"


class FunctionalExchangesConfig:
    input_path = RawExchangesConfig.output_path
    absolute_input_path = RawExchangesConfig.absolute_output_path
    output_path = f"{base_path}/functional"
    absolute_output_path = f"{exchanges_absolute_path}/functional"
    absolute_output_tmp_path = f"{absolute_output_path}/tmp"
    checkpoint_location = f"{absolute_output_path}/checkpoint"


class DatamartExchangesConfig:
    input_path = FunctionalExchangesConfig.output_path
    absolute_input_path = FunctionalExchangesConfig.absolute_output_path
    absolute_input_tmp_path = FunctionalExchangesConfig.absolute_output_tmp_path
