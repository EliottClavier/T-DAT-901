#!/bin/bash

source ./env/.env

docker cp -L spark spark-master:/opt/bitnami/spark/ &&
docker exec --env-file ./env/.env spark-master spark-submit --master spark://"${CONSUMER_RANGE}"4:"${SPARK_MASTER_PORT}" --packages org.apache.spark:spark-sql-kafka-0-10_2.12:3.5.0,org.apache.kafka:kafka-clients:2.6.0,org.apache.spark:spark-streaming-kafka-0-10_2.12:3.5.0 spark/apps/main.py
