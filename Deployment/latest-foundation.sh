#!/bin/bash

LOG_FILE="/var/log/foundation-latest.log"
exec > >(tee -a "$LOG_FILE") 2>&1

echo "======== SCRIPT RAN at $(date) ========"

PROJECT_DIR="/home/krishtopher/Documents/dentistry-foundation-blazor"
COMPOSE_FILE="$PROJECT_DIR/Deployment/docker-compose.yml"

echo "[INFO] Switching to project directory..."
cd "$PROJECT_DIR" || { echo "[ERROR] Failed to cd to $PROJECT_DIR"; exit 1; }

echo "[INFO] Pulling latest Docker images..."
docker compose -f "$COMPOSE_FILE" pull

echo "[INFO] Stopping and removing old containers..."
docker compose -f "$COMPOSE_FILE" down

echo "[INFO] Starting Docker containers..."
ASPNETCORE_ENVIRONMENT=Production DOMAIN_NAME=idental-stg-taiwan.smartsurgerytek.net  docker compose -f "$COMPOSE_FILE" up -d || {
  echo "[ERROR] Docker containers failed to start"
  exit 1
}

echo "[INFO] Update and restart successful."

