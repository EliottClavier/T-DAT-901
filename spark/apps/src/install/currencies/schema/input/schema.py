from pyspark.sql.types import StructType, StructField, StringType, DoubleType, LongType, TimestampType, IntegerType

input_schema = StructType([
    StructField("Exchange", StringType(), False),
    StructField("Name", StringType(), False),
    StructField("Price", IntegerType(), False),
    StructField("24h Volume", LongType(), False),
    StructField("Supply", IntegerType(), False),
    StructField("TimeStamp", IntegerType(), False)
])
