#FROM mcr.microsoft.com/dotnet/aspnet:8.0
#COPY bin/Release/net8.0/publish/ app/
##COPY ./src/Foundation.AuthServer/bin/Release/net8.0/publish/ app/
#
#RUN dotnet tool install -g Volo.Abp.Cli
#ENV PATH="$PATH:/root/.dotnet/tools"
#RUN abp install-libs
#
#WORKDIR /app
#ENV ASPNETCORE_URLS=http://+:80
#ENTRYPOINT ["dotnet", "Foundation.AuthServer.dll"]
### test file
# Use official ASP.NET Core runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# Set working directory
WORKDIR /app

# Copy published output from local publish directory into the container
# Make sure this path is correct relative to your Docker build context
COPY ./bin/Release/net8.0/publish/ .

# Install ABP CLI globally
RUN dotnet tool install -g Volo.Abp.Cli --version 8.1.2

# Update PATH so the ABP CLI is available
ENV PATH="${PATH}:/root/.dotnet/tools"

# Install ABP front-end and other libraries (if needed)
RUN abp install-libs

# Expose port (optional, useful for documentation or compose)
EXPOSE 80

# Set environment variable for ASP.NET Core
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "Foundation.AuthServer.dll"]
