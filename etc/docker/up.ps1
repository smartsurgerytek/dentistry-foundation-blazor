docker network create dentistryfoundationsso --label=dentistryfoundationsso
docker-compose -f docker-compose.infrastructure.yml up -d
exit $LASTEXITCODE