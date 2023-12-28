FROM bitnami/spark:latest

COPY requirements.txt /requirements.txt
RUN pip install -r /requirements.txt

CMD ["bin/spark-class", "org.apache.spark.deploy.master.Master"]