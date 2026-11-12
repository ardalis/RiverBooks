FROM mcr.microsoft.com/devcontainers/dotnet:2-11.0-resolute

# Install basic networking and DB client utilities used during local development.
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
    iputils-ping \
    netcat-openbsd \
    && rm -rf /var/lib/apt/lists/*

# The base image can lag behind .NET releases (it may still ship a preview/RC SDK),
# so install the latest GA SDK from the 11.0 channel alongside it.
RUN curl -sSL https://dot.net/v1/dotnet-install.sh -o /tmp/dotnet-install.sh \
    && bash /tmp/dotnet-install.sh --channel 11.0 --quality ga --install-dir /usr/share/dotnet \
    && rm /tmp/dotnet-install.sh
