#!/bin/bash

source ./env/.env

mkdir -p "grafana-data"
mkdir -p "spark-data"

sh ./stop_app.sh &&
docker-compose --env-file ./env/.env up --scale spark-worker=3 -d &&
docker cp -L spark spark-master:/opt/bitnami/spark/ &&
docker exec -u root spark-master chown -R 1001:1001 /opt/bitnami/spark/spark/

sleep 10

docker exec --env-file ./env/.env influxdb influx bucket create -n "metrics" -o "${DOCKER_INFLUX_DB_ORG}" -r "1w"
docker exec --env-file ./env/.env spark-master spark-submit \
      --master spark://"${CONSUMER_RANGE}"4:"${SPARK_MASTER_PORT}" \
      --packages org.apache.spark:spark-sql-kafka-0-10_2.12:3.5.0,org.apache.kafka:kafka-clients:2.6.0,org.apache.spark:spark-streaming-kafka-0-10_2.12:3.5.0,org.apache.spark:spark-sql_2.13:3.5.0 \
      --conf spark.executor.memory=2g \
      --conf spark.executor.cores=3 \
      --conf spark.executor.instances=3 \
      --conf spark.driver.memory=4g \
      --conf spark.driver.cores=4 \
      --conf "spark.driver.extraJavaOptions=-Dcom.sun.management.jmxremote -Dcom.sun.management.jmxremote.port=9999 -Dcom.sun.management.jmxremote.rmi.port=9999 -Dcom.sun.management.jmxremote.authenticate=false -Dcom.sun.management.jmxremote.ssl=false" \
      --conf "spark.executor.extraJavaOptions=-Dcom.sun.management.jmxremote -Dcom.sun.management.jmxremote.port=9999 -Dcom.sun.management.jmxremote.rmi.port=9999 -Dcom.sun.management.jmxremote.authenticate=false -Dcom.sun.management.jmxremote.ssl=false" \
      spark/apps/main.py
