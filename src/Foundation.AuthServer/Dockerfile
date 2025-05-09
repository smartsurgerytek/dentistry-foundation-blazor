FROM mcr.microsoft.com/dotnet/aspnet:8.0
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY ./bin/Release/net8.0/publish/ .
COPY .//https.pfx /https/https.pfx
RUN dotnet tool install -g Volo.Abp.Cli --version 8.1.2
ENV PATH="${PATH}:/root/.dotnet/tools"
RUN abp install-libs
ENTRYPOINT ["dotnet", "Foundation.AuthServer.dll"]
