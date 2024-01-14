import logging
from spark.apps.src.launch.MainLaunch import MainLaunch
from spark.apps.src.install.MainInstall import MainInstall

# Configuration de logging pour styliser la sortie
logger = logging.getLogger("kafka-messages")
handler = logging.StreamHandler()
handler.setFormatter(logging.Formatter('%(message)s'))
logger.addHandler(handler)
logger.setLevel(logging.INFO)


def main():
    logger.info("Starting Install")
    MainInstall().start()

    logger.info("Starting Launch")
    MainLaunch().start()


if __name__ == "__main__":
    main()
