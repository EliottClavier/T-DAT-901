from pyspark.sql.types import StructType, StructField, StringType

raw_schema = StructType([
    StructField("CurrencyName", StringType(), False),
    StructField("ExchangeName", StringType(), False),
    StructField("Price", StringType(), False),
    StructField("Volume24H", StringType(), False),
    StructField("TimeStamp", StringType(), False),
    StructField("part_dht", StringType(), False)
])
