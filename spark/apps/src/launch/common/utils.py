import os
from datetime import datetime
from typing import Optional
from pyspark.sql.functions import col, from_json


def get_dht():
    dht: Optional[str] = os.environ.get("DHT")
    return dht if dht is not None else round(datetime.now().timestamp())


def parse_kafka_df(kafka_df, schema):
    json_df = kafka_df.selectExpr("CAST(value AS STRING)")
    return json_df.select(from_json(col("value"), schema).alias("data")).select("data.*")
