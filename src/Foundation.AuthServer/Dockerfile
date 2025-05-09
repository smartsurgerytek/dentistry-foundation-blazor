FROM mcr.microsoft.com/dotnet/aspnet:8.0
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY ./bin/Release/net8.0/publish/ .
RUN dotnet tool install -g Volo.Abp.Cli --version 8.1.2
ENV PATH="${PATH}:/root/.dotnet/tools"
RUN abp install-libs
EXPOSE 80
EXPOSE 443
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "Foundation.AuthServer.dll"]
