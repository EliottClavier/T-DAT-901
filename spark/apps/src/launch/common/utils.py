import os
from datetime import datetime
from typing import Optional


def get_dht():
    dht: Optional[str] = os.environ.get("DHT")
    return dht if dht is not None else datetime.now().timestamp()
