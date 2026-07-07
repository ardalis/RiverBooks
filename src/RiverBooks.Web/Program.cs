using System.Reflection;
using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using RiverBooks.Books;
using RiverBooks.EmailSending;
using RiverBooks.Reporting;
using RiverBooks.SharedKernel;
using RiverBooks.Users;
using RiverBooks.Users.UseCases.Cart.AddItem;
using Serilog;

var logger = Log.Logger = new LoggerConfiguration()
  .Enrich.FromLogContext()
  .WriteTo.Console()
  .CreateLogger();

logger.Information("Starting web host");

var builder = WebApplication.CreateBuilder(args);

// Add Aspire service defaults (OpenTelemetry, health checks, etc.)
builder.AddServiceDefaults();

builder.Host.UseSerilog((_, config) => config.ReadFrom.Configuration(builder.Configuration));
builder.Services.AddHttpLogging(o => { });

builder.Services.AddFastEndpoints()
    .AddAuthenticationJwtBearer(s =>
    {
      s.SigningKey = builder.Configuration["Auth:JwtSecret"];
    })
    .AddAuthorization()
    .SwaggerDocument();

// Add Module Services
List<Assembly> moduleAssemblies = [typeof(Program).Assembly];
builder.Services.AddBookModuleServices(builder.Configuration, logger, moduleAssemblies);
builder.Services.AddEmailSendingModuleServices(builder.Configuration, logger, moduleAssemblies);
builder.Services.AddOrderProcessingModuleServices(builder.Configuration, logger, moduleAssemblies);
builder.Services.AddReportingModuleServices(builder.Configuration, logger, moduleAssemblies);
builder.Services.AddUsersModuleServices(builder.Configuration, logger, moduleAssemblies);

// EmailSending depends on MongoDB running
// docker run --name mongodb -d -p 27017:27017 mongo

// OrderProcessing depends on Redis running
// docker run --name my-redis -p 6379:6379 -d redis

// Set up Mediator (source generator based)
builder.Services.AddMediator(options => options.ServiceLifetime = ServiceLifetime.Scoped);
builder.Services.AddMediatorOpenTelemetryBehavior();
builder.Services.AddMediatorFluentValidationBehavior();
builder.Services.AddValidatorsFromAssemblyContaining<AddItemToCartCommandValidator>();
builder.Services.AddScoped<IDomainEventDispatcher, MediatorDomainEventDispatcher>(); // domain events

// TODO: Add a check that certain services are only registered once to avoid multiple modules 
// stepping on one another's service wirings
var app = builder.Build();

app.UseHttpLogging();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseAuthentication()
   .UseAuthorization();

app.UseFastEndpoints()
    .UseSwaggerGen();

// Map Aspire default endpoints (health checks)
app.MapDefaultEndpoints();

app.Run();

public partial class Program { }
