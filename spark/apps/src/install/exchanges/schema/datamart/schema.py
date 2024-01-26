from pyspark.sql.types import StructType, StructField, StringType, DoubleType, LongType

datamart_schema = StructType([
    StructField("TradeId", StringType(), True),
    StructField("Price", DoubleType(), True),
    StructField("Quantity", DoubleType(), True),
    StructField("MinuteAggregate", DoubleType(), True),
    StructField("CurrencySymbol", StringType(), True),
    StructField("dhi", LongType(), False),
    StructField("dht", LongType(), False)
])
