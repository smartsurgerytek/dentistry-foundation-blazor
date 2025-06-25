#!/bin/bash

# Log file location
LOG_FILE="/var/log/foundation-stop.log"

# Log the stop
{
  echo "-------------------------------------------- $(date)"
  echo "Stopping Foundation stack at $(date)"
} >> "$LOG_FILE"

# Change directory to your compose location
DEPLOY_DIR="/home/krishtopher/Documents/dentistry-foundation-blazor/Deployment"
if ! cd "$DEPLOY_DIR"; then
  echo "$(date) - ERROR: Failed to change directory to $DEPLOY_DIR." >> "$LOG_FILE"
  exit 1
fi

# Stop and remove all containers
echo "$(date) - Stopping and removing Foundation Docker containers..." >> "$LOG_FILE"
docker compose down >> "$LOG_FILE" 2>&1

# Check if it was successful
if [ $? -eq 0 ]; then
  echo "$(date) - Foundation containers stopped successfully." >> "$LOG_FILE"
else
  echo "$(date) - ERROR: Failed to stop containers." >> "$LOG_FILE"
fi

echo "-------------------------------------------- $(date)" >> "$LOG_FILE"
exit 0
