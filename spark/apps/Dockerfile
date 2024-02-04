FROM bitnami/spark:3.5.0

COPY ./metrics/metrics.properties /opt/bitnami/spark/conf/
COPY requirements.txt /requirements.txt
RUN pip install -r /requirements.txt

CMD ["bin/spark-class", "org.apache.spark.deploy.master.Master"]