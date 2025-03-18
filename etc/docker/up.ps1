docker network create foundation --label=foundation
docker-compose -f docker-compose.infrastructure.yml up -d
exit $LASTEXITCODE