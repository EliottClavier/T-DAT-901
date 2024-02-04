# T-DAT-901 - Crypto Scraper
=====================

Ce projet utilise ASP.NET Core et Selenium pour extraire en temps réel le prix de cryptomonnaie, le volume des échanges et la quantité de monnaie en circulation à partir de place et exchange, et publie ensuite ces données dans un topic Kafka.

##Prérequis
---------

-   .NET Core 6
-   Docker et Docker Compose

##Configuration et exécution
--------------------------

### 1\. Variable d'environnement

Créez un fichier .env à la racine du projet ( pas dans le dossier env qui comporte l'exemple) et entrez les informations demandées
 - L'URI du broker kafka
 - le nom du topic

### 2\. Exécution de l'application ASP.NET Core

#### Serveur local
Compilez et exécutez l'application.

shCopy code

`dotnet build
dotnet run`

#### DockerFile

Dans un terminal et si votre Dockerfile se trouve dans le répertoire courant :

shCopy code

`docker build -t app .`


#### DockerCompose

Avec cette méthode, l'ensemble des autres applications du projet seront également lancées 

Dans un terminal et si votre docker-compose.yml se trouve dans le répertoire courant :

shCopy code

`docker-compose up -d`


L'application commencera à scraper les données et à les publier dans le topic Kafka.

##Structure du projet
-------------------

-   `Api`: le projet principal contenant le point d'entrée de la solution
-   `Domain`: 
-   `Application`: 
-   `Infrastructure`:
-   `Application`: 

##Comment ça marche
-----------------

L'application utilise Selenium pour scraper les données du Bitcoin à partir de CoinMarketCap et Binance en temps réel. Les données extraites sont ensuite transformées et publiées dans un topic Kafka pour une consommation ultérieure.

##Contribution
------------