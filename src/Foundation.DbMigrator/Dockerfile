FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

COPY publish/dbmigrator/ ./

ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "Foundation.DbMigrator.dll"]
