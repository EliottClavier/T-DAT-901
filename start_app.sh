#!/bin/bash

sh ./stop_app.sh &&
docker-compose --env-file ./env/.env up -d && 
docker cp -L ./spark/apps/main.py spark-master:/opt/bitnami/spark/main.py && 
docker exec spark-master spark-submit --master spark://10.6.0.4:7077 --packages org.apache.spark:spark-sql-kafka-0-10_2.12:3.5.0,org.apache.kafka:kafka-clients:2.6.0,org.apache.spark:spark-streaming-kafka-0-10_2.12:3.5.0 main.py
