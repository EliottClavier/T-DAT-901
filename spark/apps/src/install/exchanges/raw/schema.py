from pyspark.sql.types import StructType, StructField, StringType

raw_schema = StructType([
    StructField("Price", StringType(), False),
    StructField("Quantity", StringType(), False),
    StructField("CurrencySymbol", StringType(), False),
    StructField("TimeStamp", StringType(), False),
    StructField("ExchangeName", StringType(), False),
    StructField("part_dht", StringType(), False)
])
