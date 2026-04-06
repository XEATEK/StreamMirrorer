# Base image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER root

# Install ffmpeg and streamlink
RUN apt-get update && \
    apt-get install -y --no-install-recommends \
    ffmpeg \
    streamlink && \
    rm -rf /var/lib/apt/lists/*

# Switch back to the conventional non-root user
USER app
WORKDIR /app
EXPOSE 8080

# Build image
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["StreamMirrorer/StreamMirrorer.csproj", "StreamMirrorer/"]
RUN dotnet restore "./StreamMirrorer/StreamMirrorer.csproj"
COPY . .
WORKDIR "/src/StreamMirrorer"
RUN dotnet build "./StreamMirrorer.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish image
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./StreamMirrorer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final production image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StreamMirrorer.dll"]
