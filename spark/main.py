import os
from os.path import join, dirname

from pyspark.sql import SparkSession
from pyspark.sql.types import StructType, StructField, StringType

"""
from dotenv import load_dotenv
load_dotenv('../env/.env')
"""

def main():
    spark = SparkSession.builder.appName("pyspark-app").getOrCreate()

    """
    Consume kafka topic
    """

    # df = spark.readStream.format("kafka") \
    #     .option("kafka.bootstrap.servers", "kafka:9092") \
    #     .option("subscribe", "topic") \
    #     .option("startingOffsets", "earliest") \
    #     .load()


    # Create fake schema
    schema = StructType([
        StructField("currency", StringType(), True),
        StructField("value", StringType(), True)
    ])

    df = spark.createDataFrame(spark.sparkContext.emptyRDD(), schema)

    """
    Insert treatment of the data here
    """ 

    #
    #
    #

    """
    Write to InfluxDB
    """

    df.writeStream \
        .format("influxdb") \
        .option("host", "influxdb") \
        .option("port", 8086) \
        .option("database", os.environ.get("INFLUXDB_DB")) \
        .option("measurement", "measurement") \
        .option("username", os.environ.get("INFLUXDB_ADMIN_USER")) \
        .option("password", os.environ.get("INFLUXDB_ADMIN_PASSWORD")) \
        .option("batch.mode", "true") \
        .option("batch.size", "1000") \
        .option("query", "SELECT * FROM measurement") \
        .start()
    
    """
    Start the stream
    """
    df.awaitTermination();



if __name__ == '__main__':
    main()