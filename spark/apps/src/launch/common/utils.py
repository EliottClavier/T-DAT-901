import os
from datetime import datetime


def get_dht():
    return os.environ["DHT"] if os.environ["DHT"] is not None else datetime.now().timestamp()