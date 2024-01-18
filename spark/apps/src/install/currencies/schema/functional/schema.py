from pyspark.sql.types import StructType, StructField, StringType, DoubleType, LongType

functional_schema = StructType([
    StructField("CurrencyName", StringType(), False),
    StructField("ExchangeName", StringType(), False),
    StructField("Price", DoubleType(), False),
    StructField("part_dhi", LongType(), False),
    StructField("dht", LongType(), False)
])
