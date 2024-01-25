FROM bitnami/spark:3.5.0

COPY requirements.txt /requirements.txt
RUN pip install -r /requirements.txt

CMD ["bin/spark-class", "org.apache.spark.deploy.master.Master"]