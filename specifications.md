# Spécifications liées au projet T-DAT-900

# Table des Matières
1. [Sources de données](#sources-de-données)
2. [Quelles cryptomonnaies ?](#quelles-cryptomonnaies-)
3. [Structure](#structure-quelles-données-remontées-par-le-scrapper)
4. [Traitement Spark](#traitement-spark)
5. [Métriques à afficher](#métriques-à-afficher)
6. [Liens utiles](#liens-utiles)

## Sources de données
- [Binance](https://www.binance.com/fr/markets/overview)
- [Blockchain](https://exchange.blockchain.com/)
- [Coinbase](https://www.coinbase.com/fr/explore)
- [Crypto](https://crypto.com/price)

## Quelles cryptomonnaies ?
- Bitcoin
- Etherum

## Structure (quelles données remontées par le Scrapper)
- Nom
- Valeur achat
- Volume 24h
- Timestamp

## Traitement Spark
- Calcul du volume sur une durée supérieure à 24h
- Moyenne des valeurs d’achats / plusieurs sources

## Métriques à afficher
- Volume sur une durée supérieure à 24h
- Graphique avec courbes de valeurs ?
- Graphique avec courbes de valeurs / sources pour une monnaie
- Valeur d’achat brute actuelle
- Indicateur de tendance
- Analyse de courbes ?


