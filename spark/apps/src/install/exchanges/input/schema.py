from pyspark.sql.types import StructType, StructField, StringType, DoubleType, IntegerType

input_schema = StructType([
    StructField("Price", IntegerType(), False),
    StructField("Quantity", DoubleType(), False),
    StructField("CurrencySymbol", StringType(), False),
    StructField("TimeStamp", IntegerType(), False),
    StructField("ExchangeName", StringType(), False)
])
