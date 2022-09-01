#!/bin/bash

SCRIPT_PATH=$( cd "$(dirname "${BASH_SOURCE[0]}")" ; pwd -P )
if [ ${1} == "up" ]
then
    docker-compose --file=${SCRIPT_PATH}/../docker/docker-compose.yaml build
    docker-compose --file=${SCRIPT_PATH}/../docker/docker-compose.yaml up -d
elif [ ${1} == "down" ]
then
    docker-compose --file=${SCRIPT_PATH}/../docker/docker-compose.yaml down
elif [ ${1} == "restart" ]
then
    docker-compose --file=${SCRIPT_PATH}/../docker/docker-compose.yaml build
    docker-compose --file=${SCRIPT_PATH}/../docker/docker-compose.yaml up -d
    docker-compose --file=${SCRIPT_PATH}/../docker/docker-compose.yaml down
else
    echo "up or down or restart option must be provided"
fi