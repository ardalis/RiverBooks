#!/usr/bin/env bash
set -euo pipefail

cd /workspaces/RiverBooks

echo "Restoring local .NET tools..."
dotnet tool restore

echo "Restoring solution packages..."
cd src
dotnet restore RiverBooks.slnx
