from pyspark.sql.types import StructType, StructField, StringType, DoubleType, TimestampType, IntegerType, LongType

functional_schema = StructType([
    StructField("TradeId", StringType(), True),
    StructField("Price", DoubleType(), True),
    StructField("Quantity", DoubleType(), True),
    StructField("CurrencySymbol", StringType(), True),
    StructField("dhi", LongType(), False),
    StructField("dht", LongType(), False)
])



