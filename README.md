# T-DAT-901 - Crypto Scraper
=====================

Ce projet utilise ASP.NET Core et Selenium pour extraire en temps réel le prix du Bitcoin, le volume des échanges et la quantité de monnaie en circulation à partir de CoinMarketCap, et publie ensuite ces données dans un topic Kafka.

##Prérequis
---------

-   .NET Core 3.1 ou version ultérieure
-   Docker et Docker Compose pour Kafka et Zookeeper
-   Un navigateur compatible avec Selenium, comme Chrome

##Configuration et exécution
--------------------------

### 1\. Kafka et Zookeeper avec Docker

Déployez Kafka et Zookeeper à l'aide de Docker Compose.

shCopy code

`docker-compose up -d`

### 2\. Création d'un topic Kafka

Créez un topic Kafka pour stocker les données de Bitcoin. Assurez-vous de remplacer `[KAFKA_CONTAINER_ID]` par l'ID de votre conteneur Kafka.

shCopy code

`docker exec -it [KAFKA_CONTAINER_ID] kafka-topics.sh --create --zookeeper zookeeper:2181 --replication-factor 1 --partitions 1 --topic bitcoin-info`

### 3\. Exécution de l'application ASP.NET Core

Compilez et exécutez l'application.

shCopy code

`dotnet build
dotnet run`

L'application commencera à scraper les données et à les publier dans le topic Kafka.

##Structure du projet
-------------------

-   `BitcoinScraper`: le projet principal contenant le code du scraper et la publication Kafka.
-   `BitcoinScraper.Tests`: projet de tests pour le scraper.
-   `docker-compose.yml`: fichier de configuration pour déployer Kafka et Zookeeper avec Docker.

##Comment ça marche
-----------------

L'application utilise Selenium pour scraper les données du Bitcoin à partir de CoinMarketCap en temps réel. Les données extraites sont ensuite transformées et publiées dans un topic Kafka pour une consommation ultérieure.

##Contribution
------------