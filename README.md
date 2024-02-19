# RiverBooks

## EF Migration Scripts

```dotnetcli
dotnet tool install --global dotnet-ef
dotnet tool update --global dotnet-ef

dotnet add package Microsoft.EntityFrameworkCore.Design

dotnet ef migrations add Initial -c BookDbContext -p ../RiverBooks.Books/RiverBooks.Books.csproj -s RiverBooks.Web.csproj -o Data/Migrations

dotnet ef database update
```

Watch out for `<InvariantGlobalization>true</InvariantGlobalization>` in your Web API project.

Once you have multiple modules you need so specify the context every time:

```dotnetcli
dotnet ef migrations add CartItemDescription -c UsersDbContext -p ..\RiverBooks.Users\RiverBooks.Users.csproj -s .\RiverBooks.Web.csproj -o Data/Migrations

dotnet ef database update -c UsersDbContext
```

For Order Processing:

```dotnetcli
dotnet ef migrations add Initial_OrderProcessing -c OrderProcessingDbContext -p ..\RiverBooks.OrderProcessing\RiverBooks.OrderProcessing.csproj -s .\RiverBooks.Web.csproj -o Data/Migrations

dotnet ef database update -c OrderProcessingDbContext
```

Adding Addresses to Users:

```dotnetcli
dotnet ef migrations add UserAddresses -c UsersDbContext -p ..\RiverBooks.Users\RiverBooks.Users.csproj -s .\RiverBooks.Web.csproj -o Data/Migrations

dotnet ef database update -c UsersDbContext
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
