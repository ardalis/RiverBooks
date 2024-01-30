using FastEndpoints;
using FastEndpoints.Swagger;
using RiverBooks.Books;
using Serilog;

var logger = Log.Logger = new LoggerConfiguration()
  .Enrich.FromLogContext()
  .WriteTo.Console()
  .CreateLogger();

logger.Information("Starting web host");

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((_, config) => config.ReadFrom.Configuration(builder.Configuration));

builder.Services.AddFastEndpoints()
    .SwaggerDocument();

// Add Module Services
builder.Services.AddBookServices(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();

app.UseFastEndpoints()
    .UseSwaggerGen();

app.Run();

public partial class Program { }
