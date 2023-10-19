#!/bin/bash

# Récupérer l'ID du conteneur du Spark Master
MASTER_CONTAINER_ID=$(docker ps -qf "name=spark-master")

# Copier le fichier Python dans le conteneur du Spark Master
docker cp -L ./spark/apps/main.py $MASTER_CONTAINER_ID:/opt/bitnami/spark/main.py

# Exécuter spark-submit dans le conteneur du Spark Master
docker exec $MASTER_CONTAINER_ID spark-submit --master spark://10.6.0.4:7077 --packages org.apache.spark:spark-sql-kafka-0-10_2.12:3.5.0,org.apache.kafka:kafka-clients:2.6.0,org.apache.spark:spark-streaming-kafka-0-10_2.12:3.5.0 main.py
