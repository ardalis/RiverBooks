# RiverBooks

Sample application for [Modular Monolith Courses on Dometrain](https://dometrain.com/author/steve-ardalis-smith/). Use 'ARDALIS' for 20% off individual courses!

📚 **[View Full Documentation](https://riverbooks.ardalis.com/)**

## Architecture Overview

[View the interactive dependency diagram](https://riverbooks.ardalis.com/docs/architecture/dependencies-diagram/#interactive-dependency-diagram)

Project Dependencies

## Updates

**2025 December** Updated to .NET 10 and replaced MediatR with [Mediator.SourceGen](https://github.com/martinothamar/Mediator).
**2025 July** Removed FluentAssertions and replaced with [Shoudly](https://github.com/shouldly/shouldly).

## EF Migration Scripts

```dotnetcli
dotnet tool install --global dotnet-ef
dotnet tool update --global dotnet-ef

dotnet add package Microsoft.EntityFrameworkCore.Design

dotnet ef migrations add Initial -c BookDbContext -p ../RiverBooks.Books/RiverBooks.Books.csproj -s RiverBooks.Web.csproj -o Data/Migrations

dotnet ef database update -c BookDbContext

-- You may need to specify projects
dotnet ef database update -c BookDbContext -p .\RiverBooks.Books.csproj --startup-project ..\RiverBooks.Web\RiverBooks.Web.csproj

-- for tests to work
dotnet ef database update -c BookDbContext -p .\RiverBooks.Books.csproj --startup-project ..\RiverBooks.Web\RiverBooks.Web.csproj -- --environment Testing
```

Watch out for `<InvariantGlobalization>true</InvariantGlobalization>` in your Web API project.

Once you have multiple modules you need so specify the context every time:

```dotnetcli
-- in RiverBooks.Users folder:
dotnet ef migrations add CartItemDescription -c UsersDbContext -p ..\RiverBooks.Users\RiverBooks.Users.csproj -s .\RiverBooks.Web.csproj -o Data/Migrations

dotnet ef database update -c UsersDbContext -p .\RiverBooks.Users.csproj -s ..\RiverBooks.Web\RiverBooks.Web.csproj
```

For Order Processing:

```dotnetcli
-- in RiverBooks.OrderProcessing folder:
dotnet ef migrations add Initial_OrderProcessing -c OrderProcessingDbContext -p ..\RiverBooks.OrderProcessing\RiverBooks.OrderProcessing.csproj -s .\RiverBooks.Web.csproj -o Data/Migrations

dotnet ef database update -c OrderProcessingDbContext -p RiverBooks.OrderProcessing.csproj -s ..\RiverBooks.Web\RiverBooks.Web.csproj
```

Adding Addresses to Users:

```dotnetcli
-- in RiverBooks.Users folder:
dotnet ef migrations add UserAddresses -c UsersDbContext -p ..\RiverBooks.Users\RiverBooks.Users.csproj -s .\RiverBooks.Web.csproj -o Data/Migrations

dotnet ef database update -c UsersDbContext -p ..\RiverBooks.Users\RiverBooks.Users.csproj -s .\RiverBooks.Web.csproj
```

## Local Tools and Slopwatch

Restore the repo-local .NET tools before running repository tooling:

```dotnetcli
dotnet tool restore
```

Run Slopwatch to detect newly introduced disabled tests, warning suppressions, empty catch blocks, and similar shortcut-style changes:

```dotnetcli
dotnet tool run slopwatch analyze -d . --fail-on warning
```

If Slopwatch reports that the baseline is missing, initialize it once and commit the generated file:

```dotnetcli
dotnet tool run slopwatch init
```

## Docker Commands

### Redis

```bash
docker run --name my-redis -p 6379:6379 -d redis
```

### MongoDB

```bash
docker run --name mongodb -d -p 27017:27017 mongo
```

### Papercut

```bash
docker run --name=papercut -p 25:25 -p 37408:37408 jijiechen/papercut:latest -d
```
