#!/bin/bash

# Log file location
LOG_FILE="/var/log/foundation-start.log"

# Log the start
{
  echo "-------------------------------------------- $(date)"
  echo "Starting Foundation stack at $(date)"
  echo "$(date) - Waiting for Docker to initialize..."
} >> "$LOG_FILE"

# Wait for Docker to be fully ready
sleep 30

# Change directory to your compose location
DEPLOY_DIR="/home/krishtopher/Documents/dentistry-foundation-blazor/Deployment"
if ! cd "$DEPLOY_DIR"; then
  echo "$(date) - ERROR: Failed to change directory to $DEPLOY_DIR." >> "$LOG_FILE"
  exit 1
fi

# Clean up any existing container using Redis port 6379
echo "$(date) - Checking and cleaning old Redis container on port 6379..." >> "$LOG_FILE"
docker ps -q --filter "publish=6379" | xargs -r docker rm -f >> "$LOG_FILE" 2>&1

# Down all running docker containers
echo "$(date) - Removing Foundation Docker containers..." >> "$LOG_FILE"
docker compose down >> "$LOG_FILE" 2>&1

# Start Docker Compose services
echo "$(date) - Starting Foundation Docker containers..." >> "$LOG_FILE"
ASPNETCORE_ENVIRONMENT=Production DOMAIN_NAME=idental-stg-taiwan.smartsurgerytek.net docker compose up -d >> "$LOG_FILE" 2>&1

# Check if it was successful
if [ $? -eq 0 ]; then
  echo "$(date) - Foundation containers started successfully." >> "$LOG_FILE"
else
  echo "$(date) - ERROR: Failed to start containers." >> "$LOG_FILE"
fi

echo "-------------------------------------------- $(date)" >> "$LOG_FILE"
exit 0

