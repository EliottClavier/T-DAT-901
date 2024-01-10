from pyspark.sql.types import StructType, StructField, StringType

raw_schema = StructType([
    StructField("Exchange", StringType(), False),
    StructField("Name", StringType(), False),
    StructField("Price", StringType(), False),
    StructField("24h Volume", StringType(), False),
    StructField("Supply", StringType(), False),
    StructField("TimeStamp", StringType(), False),
    StructField("part_dht", StringType(), False)
])
