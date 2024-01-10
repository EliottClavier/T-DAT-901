from pyspark.sql.types import StructType, StructField, StringType, DoubleType, TimestampType, IntegerType

functional_schema = StructType([
    StructField("Price", IntegerType(), False),
    StructField("Quantity", DoubleType(), False),
    StructField("CurrencySymbol", StringType(), False),
    StructField("TimeStamp", TimestampType(), False),
    StructField("ExchangeName", StringType(), False),
    StructField("part_dht", TimestampType(), False)
])
