FROM mcr.microsoft.com/dotnet/aspnet:8.0
COPY bin/Release/net8.0/publish/ app/
#COPY ./src/Foundation.HttpApi.Host/bin/Release/net8.0/publish/ app/
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "Foundation.HttpApi.Host.dll"]