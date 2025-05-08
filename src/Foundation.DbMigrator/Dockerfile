#FROM mcr.microsoft.com/dotnet/aspnet:8.0
#COPY bin/Release/net8.0/publish/ app/
##COPY ./src/Foundation.DbMigrator/bin/Release/net8.0/publish/ app/
#WORKDIR /app
#ENV ASPNETCORE_URLS=http://+:80
#ENTRYPOINT ["dotnet", "Foundation.DbMigrator.dll"]

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:80

COPY ./publish/ .

ENTRYPOINT ["dotnet", "Foundation.DbMigrator.dll"]
