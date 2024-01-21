from pyspark.sql.types import StructType, StructField, StringType, DoubleType, LongType

datamart_schema = StructType([
    StructField("CurrencyName", StringType(), False),
    StructField("ExchangeName", StringType(), False),
    StructField("Price", DoubleType(), True),
    StructField("dhi", LongType(), False),
    StructField("dht", LongType(), False)
])
