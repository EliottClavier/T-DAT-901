# Use an official Python runtime as a parent image
FROM python:3.11.5-slim

# Install required system packages
RUN apt-get install software-properties-common -y && \
    add-apt-repository ppa:openjdk-r/ppa && \
    apt-get update && \
    apt-get install -y openjdk-11-jre-headless && \
    apt-get clean

# Install PySpark and any other Python dependencies
RUN pip install pyspark

# Copy the entire project directory into the container
COPY . /app

# Set the working directory to /app
WORKDIR /app

# Specify the command to run your PySpark application
CMD ["python", "main.py"]
