from pyspark.sql.types import StructType, StructField, StringType, DoubleType, IntegerType, LongType

input_schema = StructType([
    StructField("TradeId", StringType(), False),
    StructField("Price", StringType(), False),
    StructField("Quantity", StringType(), False),
    StructField("CurrencySymbol", StringType(), False),
    StructField("TimeStamp", StringType(), False),
    StructField("ExchangeName", StringType(), False)
])

