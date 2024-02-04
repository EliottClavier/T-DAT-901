#!/bin/bash
DOCKER_SOCK_GID=$(stat -c '%g' /var/run/docker.sock)
groupmod -g "$DOCKER_SOCK_GID" telegraf

# Assurez-vous que l'utilisateur 'telegraf' a les bons droits
chown telegraf:telegraf /var/run/docker.sock

# Démarrage de Telegraf
exec telegraf