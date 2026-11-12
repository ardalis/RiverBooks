---
title: "Overview"
description: "Set up your development environment and run RiverBooks"
summary: "Learn how to set up and run the RiverBooks application"
date: 2024-12-04
weight: 1
---

Get up and running with RiverBooks in just a few minutes.

## Prerequisites

Before you begin, ensure you have the following installed:

- [.NET 11 SDK](https://dotnet.microsoft.com/download) or later
- [SQL Server](https://www.microsoft.com/sql-server) (LocalDB, Express, or full version)
- [MongoDB](https://www.mongodb.com/try/download/community) (for email outbox)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

## Clone the Repository

```bash
git clone https://github.com/ardalis/RiverBooks.git
cd RiverBooks
```

## Build the Solution

```bash
cd src
dotnet build
```

## Database Setup

RiverBooks uses Entity Framework Core with SQL Server. You'll need to run migrations for each module's database context.

### Install EF Core Tools

```bash
dotnet tool install --global dotnet-ef
```

Or update if already installed:

```bash
dotnet tool update --global dotnet-ef
```

### Run Migrations

From the `src` folder, run migrations for each context:

```bash
# Books module
dotnet ef database update -c BookDbContext -p .\RiverBooks.Books\RiverBooks.Books.csproj --startup-project .\RiverBooks.Web\RiverBooks.Web.csproj

# Users module  
dotnet ef database update -c UsersDbContext -p .\RiverBooks.Users\RiverBooks.Users.csproj -s .\RiverBooks.Web\RiverBooks.Web.csproj

# Order Processing module
dotnet ef database update -c OrderProcessingDbContext -p .\RiverBooks.OrderProcessing\RiverBooks.OrderProcessing.csproj -s .\RiverBooks.Web\RiverBooks.Web.csproj
```

### For Test Environment

```bash
dotnet ef database update -c BookDbContext -p .\RiverBooks.Books\RiverBooks.Books.csproj --startup-project .\RiverBooks.Web\RiverBooks.Web.csproj -- --environment Testing
```

## Configuration

The application uses standard ASP.NET Core configuration. Key settings are in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=RiverBooks;..."
  },
  "MongoDBSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "RiverBooksEmail"
  }
}
```

## Run the Application

```bash
cd src/RiverBooks.Web
dotnet run
```

The API will be available at `https://localhost:7xxx` (port shown in console output).

## Swagger UI

Navigate to `/swagger` to explore the API endpoints using Swagger UI.

## Next Steps

- [Architecture](/docs/architecture/) - Learn about the modular architecture
- [EF Migrations](/docs/getting-started/ef-migrations/) - Detailed migration commands
