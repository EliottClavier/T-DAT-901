from pyspark.sql.types import StructType, StructField, StringType

raw_schema = StructType([
    StructField("TradeId", StringType(), False),
    StructField("Price", StringType(), False),
    StructField("Quantity", StringType(), False),
    StructField("CurrencySymbol", StringType(), False),
    StructField("TimeStamp", StringType(), False),
    StructField("part_dht", StringType(), False)
])
