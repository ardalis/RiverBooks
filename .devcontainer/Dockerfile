FROM mcr.microsoft.com/devcontainers/dotnet:1-10-bookworm

# Install basic networking and DB client utilities used during local development.
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
    iputils-ping \
    netcat-openbsd \
    && rm -rf /var/lib/apt/lists/*
