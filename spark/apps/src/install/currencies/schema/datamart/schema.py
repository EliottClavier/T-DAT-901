from pyspark.sql.types import StructType, StructField, StringType, DoubleType, LongType

candle_schema = StructType([
    StructField("CurrencyName", StringType(), True),
    StructField("dhi", LongType(), True),
    StructField("dht", LongType(), True),
    StructField("minuteCandle", StringType(), True)
])

datamart_schema = StructType([
    StructField("CurrencyName", StringType(), False),
    StructField("ExchangeName", StringType(), False),
    StructField("Price", DoubleType(), True),
    StructField("minuteCandle", StringType(), True),
    StructField("dhi", LongType(), False),
    StructField("dht", LongType(), False)
])
