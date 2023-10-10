# Use an official Python runtime as a parent image
FROM python:3.11.5-slim



# Set environment variables to non-interactive (this prevents some prompts)
ENV DEBIAN_FRONTEND=noninteractive

	# Install required system packages
RUN apt-get update && \
    apt-get install -y openjdk-17-jre-headless software-properties-common && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

# Verify Java installation
RUN java -version

# Install PySpark and any other Python dependencies
RUN pip install pyspark

# Copy the entire project directory into the container
COPY . /app

# Set the working directory to /app
WORKDIR /app

# Specify the command to run your PySpark application
CMD ["python", "main.py"]
