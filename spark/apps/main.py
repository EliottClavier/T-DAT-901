from pyspark.sql import SparkSession
import logging

# Configuration de logging pour styliser la sortie
logger = logging.getLogger("kafka-messages")
handler = logging.StreamHandler()
handler.setFormatter(logging.Formatter('%(message)s'))
logger.addHandler(handler)
logger.setLevel(logging.INFO)


def stylized_log(message):
    logger.info("===" * 10)
    logger.info(message)
    logger.info("===" * 10)


def main():
    spark = SparkSession.builder \
        .appName("KafkaPySparkStreaming") \
        .getOrCreate()

    raw_stream = spark.readStream \
        .format("kafka") \
        .option("kafka.bootstrap.servers", "kafka:9092") \
        .option("subscribe", "crypto_topic") \
        .load()

    string_stream = raw_stream.selectExpr("CAST(value AS STRING) as value")

    # À chaque fois qu'une donnée est reçue, la log avec un style personnalisé
    def foreach_batch_function(df, epoch_id):
        messages = df.collect()
        for row in messages:
            stylized_log(row["value"])

    query = string_stream.writeStream.foreachBatch(foreach_batch_function).outputMode("append").start()

    query.awaitTermination()


if __name__ == "__main__":
    main()