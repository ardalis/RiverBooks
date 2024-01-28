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

