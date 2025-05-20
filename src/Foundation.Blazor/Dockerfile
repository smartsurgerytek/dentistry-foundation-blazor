FROM mcr.microsoft.com/dotnet/aspnet:8.0
COPY bin/Release/net8.0/publish/ app/
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "Foundation.Blazor.dll"]