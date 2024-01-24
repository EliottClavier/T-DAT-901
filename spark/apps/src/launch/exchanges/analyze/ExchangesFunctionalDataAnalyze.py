from pyspark.sql.functions import col
from datetime import datetime
from spark.apps.src.config.SparkSessionCustom import SparkSessionCustom
from spark.apps.src.install.exchanges.schema.functional.schema import functional_schema
from spark.apps.src.install.exchanges.schema.raw.schema import raw_schema
from spark.apps.src.launch.exchanges.LaunchExchangesConfig import FunctionalExchangesConfig as config


class ExchangesFunctionalDataAnalyze(SparkSessionCustom):
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
            .parquet(config.absolute_input_path)

    def write_partitioned_row(self, row):
        timestamp_seconds = row['dhi'] / 1000
        dhi = datetime.fromtimestamp(timestamp_seconds)
        output_row_df = self.spark.createDataFrame([row], functional_schema)
        output_row_df \
            .drop("dhi") \
            .write \
            .mode("append") \
            .parquet(f"{config.absolute_output_path}/dhi={dhi.strftime('%Y%m%d%H%M')}")

    def transform(self, input_df, epoch_id):
        input_df = (input_df
                    .withColumnRenamed("part_dht", "dht")
                    .withColumnRenamed("TimeStamp", "dhi"))

        output_df = input_df.select([col(field.name)
                                    .cast(field.dataType)
                                    .alias(field.name) for field in functional_schema.fields])

        for row in output_df.collect():
            self.write_partitioned_row(row)

        output_df.write \
            .mode("append") \
            .parquet(config.absolute_output_tmp_path)
