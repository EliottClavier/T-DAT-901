from pyspark.sql.types import StructType, StructField, StringType

input_schema = StructType([
    StructField("CurrencyName", StringType(), False),
    StructField("ExchangeName", StringType(), False),
    StructField("Price", StringType(), True),
    StructField("TimeStamp", StringType(), False)
])