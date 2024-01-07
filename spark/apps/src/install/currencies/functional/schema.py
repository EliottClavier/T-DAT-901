from pyspark.sql.types import StructType, StructField, StringType, DoubleType, LongType, TimestampType

functional_schema = StructType([
    StructField("Exchange", StringType(), False),
    StructField("Name", StringType(), False),
    StructField("Price", DoubleType(), False),
    StructField("24h Volume", LongType(), False),
    StructField("Supply", LongType(), False),
    StructField("TimeStamp", TimestampType(), False),
    StructField("dht", TimestampType(), False)
])
