import datetime
import os
import logging

from pyspark.sql import SparkSession
from pyspark.sql.functions import col, struct
from influxdb_client import InfluxDBClient, Point
from influxdb_client.client.write_api import SYNCHRONOUS

# Configuration de logging pour styliser la sortie
logger = logging.getLogger("kafka-messages")
handler = logging.StreamHandler()
handler.setFormatter(logging.Formatter('%(message)s'))
logger.addHandler(handler)
logger.setLevel(logging.INFO)

class SparkStreaming:

    # Spark
    spark: SparkSession = None
    raw_stream = None
    string_stream = None

    # InfluxDB
    client: InfluxDBClient = None
    measurement = "CryptoCurrency"

    def __init__(self):

        # Initialize Spark
        self.spark = SparkSession.builder \
            .appName("KafkaPySparkStreaming") \
            .getOrCreate()

        self.raw_stream = self.spark.readStream \
            .format("kafka") \
            .option("kafka.bootstrap.servers", os.environ["KAFKA_BOOTSTRAP_SERVERS"]) \
            .option("subscribe", os.environ["KAFKA_DEFAULT_TOPIC"]) \
            .load()

        self.string_stream = self.raw_stream.selectExpr("CAST(value AS STRING) as value")

    def write_to_influxdb(self, pandas_df):

        client = InfluxDBClient(
            url=f"http://{os.environ['INFLUXDB_HOST']}:{os.environ['INFLUXDB_PORT']}",
            token=os.environ["INFLUXDB_ADMIN_TOKEN"],
            org=os.environ["INFLUXDB_ORG"]
        )

        write_api = client.write_api(write_options=SYNCHRONOUS)

        for index, row in pandas_df.iterrows():
            print(row)
            print(row)
            print(row)

            # String to timestamp
            row["TimeStamp"] = datetime.datetime.fromtimestamp(row["TimeStamp"])

            point = (
                Point(self.measurement)
                .time(row["TimeStamp"])
                .tag("CurrencyName", row["CurrencyName"])
                .field("CurrencyPair", row["CurrencyPair"])
                .field("CurrencySymbol", row["CurrencySymbol"])
                .field("ExchangeName", row["ExchangeName"])
                .field("Price", row["Price"])
                .field("Volume24H", row["Volume24H"])
                #.field("CirculatingSupply", row["CirculatingSupply"])
                .field("Liquidity", row["Liquidity"])
            )

            write_api.write(bucket=os.environ["INFLUXDB_BUCKET"], org=os.environ["INFLUXDB_ORG"], record=point)

def main():
    spark_streaming = SparkStreaming()

    def foreach_batch_function(df, epoch_id):
        # Transform value into df with all columns in string json of value
        df = df.withColumn("value", df["value"].cast("string"))
        df = df.selectExpr("CAST(value AS STRING) as value")
        df = df.selectExpr("from_json(value, 'CurrencyName STRING, CurrencyPair STRING, CurrencySymbol STRING, ExchangeName STRING, Price STRING, Volume24H STRING, CirculatingSupply STRING, Liquidity STRING, TimeStamp STRING') AS value")

        # Transform Price, Volume24H, CirculatingSupply, Liquidity into float
        df = df.withColumn("value", struct(
            "value.CurrencyName",
            "value.CurrencyPair",
            "value.CurrencySymbol",
            "value.ExchangeName",
            col("value.Price").cast("float").alias("Price"),
            col("value.Volume24H").cast("int").alias("Volume24H"),
            #col("value.CirculatingSupply").cast("int").alias("CirculatingSupply"),
            col("value.Liquidity").cast("int").alias("Liquidity"),
            col("value.TimeStamp").cast("int").alias("TimeStamp")
        ))

        df = df.select("value.*")

        # To pandas df
        pandas_df = df.toPandas()

        # Write to InfluxDB
        spark_streaming.write_to_influxdb(pandas_df)

    query = spark_streaming.string_stream.writeStream.foreachBatch(foreach_batch_function).outputMode("append").start()

    query.awaitTermination()


if __name__ == "__main__":
    main()