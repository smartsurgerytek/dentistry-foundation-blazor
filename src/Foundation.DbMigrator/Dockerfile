FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

# Copy the published output from the host build context
COPY src/Foundation.DbMigrator/bin/Release/net8.0/publish/ ./

ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "Foundation.DbMigrator.dll"]
