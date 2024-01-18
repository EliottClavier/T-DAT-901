from pyspark.sql.functions import col
from spark.apps.src.install.currencies.schema.functional.schema import functional_schema
from spark.apps.src.install.currencies.schema.raw.schema import raw_schema
from spark.apps.src.launch.currencies.LaunchCurrenciesConfig import FunctionalCurrenciesConfig as config


class CurrenciesFunctionalDataAnalyze:

    def __init__(self, spark):
        self.spark = spark
        self.functional_stream = self.read_from_parquet()

    def start(self):
        self.functional_stream.writeStream \
            .foreachBatch(self.transform) \
            .start()

    def read_from_parquet(self):
        return self.spark.readStream \
            .schema(raw_schema) \
            .option("cleanSource", "delete") \
            .json(config.absolute_input_path)

    def transform(self, input_df, epoch_id):
        input_df = (input_df
                    .withColumnRenamed("part_dht", "dht")
                    .withColumnRenamed("Timestamp", "part_dhi"))

        output_df = input_df.select([col(field.name)
                                    .cast(field.dataType)
                                    .alias(field.name) for field in functional_schema.fields])

        return (output_df.write
                .mode("append")
                .json(config.absolute_output_path))
