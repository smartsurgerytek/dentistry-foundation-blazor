# Use official ASP.NET Core runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Set working directory for runtime
WORKDIR /app

# Arguments passed during build
ARG CERT_BASE64=""
ARG CERT_PASSWORD="Krish@3003"

# Decode the certificate from base64 to a file
RUN if [ ! -z "$CERT_BASE64" ]; then \
        mkdir -p /https && \
        echo "$CERT_BASE64" | base64 -d > /https/authserver.pfx; \
    fi

# Set Kestrel to use the certificate
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/authserver.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=$CERT_PASSWORD

# Set environment variable for ASP.NET Core
ENV ASPNETCORE_URLS=http://+:80

# Expose ports
EXPOSE 80
EXPOSE 443

# --- Build stage ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set working directory
WORKDIR /app

# Copy published output from local publish directory into the container
COPY ./bin/Release/net8.0/publish/ .

# Install ABP CLI globally
RUN dotnet tool install -g Volo.Abp.Cli --version 8.1.2

# Update PATH so the ABP CLI is available
ENV PATH="${PATH}:/root/.dotnet/tools"

# Install ABP front-end and other libraries (if needed)
RUN abp install-libs

# Final entry point
ENTRYPOINT ["dotnet", "Foundation.AuthServer.dll"]
