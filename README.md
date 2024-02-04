# Projet d'Analyse de Cryptomonnaie

Ce projet utilise Apache Spark pour analyser les données de cryptomonnaie en temps réel. Il traite les données streamées depuis Kafka, les analyse, puis stocke les résultats dans InfluxDB pour la visualisation avec Grafana.

## Sommaire

1. [Prérequis](#prérequis)
2. [Architecture du Projet](#architecture-du-projet)
3. [Fonctionnement](#fonctionnement)
4. [Exécution](#exécution)
   - [Variables d'environnement](#variables-denvironnement)
   - [Lancement des services](#lancement-des-services)
5. [URL des services](#url-des-services)

## Prérequis

- Docker et Docker Compose

## Architecture du Projet

- **Application Producer** : Lire les données de cryptomonnaie à partir de différentes sources et les publier dans un topic Kafka.
- **Spark Master** : Coordonne la distribution des tâches et la gestion des workers Spark.
- **Spark Workers** : Exécutent les tâches de traitement des données assignées par le Spark Master.
- **InfluxDB** : Base de données de séries temporelles utilisée pour le stockage des résultats d'analyse.
- **Grafana** : Outil de visualisation connecté à InfluxDB pour afficher les résultats des analyses.

## Fonctionnement

Le producteur de données extrait les données de cryptomonnaie à partir de différentes sources et les publie dans un topic Kafka.
L'application Spark lit en continu les données de Kafka, effectue des analyses et des agrégations, puis écrit les résultats dans InfluxDB. Ces données peuvent ensuite être visualisées et explorées à l'aide de tableaux de bord dans Grafana.

## Exécution

### Variables d'environnement

Nommez le fichier `.env.example` en `.env` et remplissez les informations demandées.

### Lancement des services

Pour lancer l'ensemble des services, exécutez le script `start_app.sh` à la racine du projet.

```sh
sh ./start_app.sh
```

## URL des services

- **Kafka** : [localhost:9092](http://localhost:9092)
- **Zookeeper** : [localhost:2181](http://localhost:2181)
- **Spark Master** : [localhost:8080](http://localhost:8080)
- **Spark Workers** : [localhost:8081](http://localhost:8081)
- **InfluxDB** : [localhost:8086](http://localhost:8086)
- **Grafana** : [localhost:3000](http://localhost:3000)