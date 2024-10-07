using System.Reflection;
using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using RiverBooks.Books;
using RiverBooks.Reporting;
using RiverBooks.Users;
using RiverBooks.SharedKernel;
using Serilog;
using RiverBooks.Users.UseCases.Cart.AddItem;
using RiverBooks.EmailSending;

var logger = Log.Logger = new LoggerConfiguration()
  .Enrich.FromLogContext()
  .WriteTo.Console()
  .CreateLogger();

logger.Information("Starting web host");

var builder = WebApplication.CreateBuilder(args);

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
List<Assembly> mediatRAssemblies = [typeof(Program).Assembly];
builder.Services.AddBookModuleServices(builder.Configuration, logger, mediatRAssemblies);
builder.Services.AddEmailSendingModuleServices(builder.Configuration, logger, mediatRAssemblies);
builder.Services.AddOrderProcessingModuleServices(builder.Configuration, logger, mediatRAssemblies);
builder.Services.AddReportingModuleServices(builder.Configuration, logger, mediatRAssemblies);
builder.Services.AddUsersModuleServices(builder.Configuration, logger, mediatRAssemblies);

// EmailSending depends on MongoDB running
// docker run --name mongodb -d -p 27017:27017 mongo

// OrderProcessing depends on Redis running
// docker run --name my-redis -p 6379:6379 -d redis

// Set up MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(mediatRAssemblies.ToArray()));
builder.Services.AddMediatRLoggingBehavior();
builder.Services.AddMediatRFluentValidationBehavior();
builder.Services.AddValidatorsFromAssemblyContaining<AddItemToCartCommandValidator>();
builder.Services.AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>(); // domain events


var app = builder.Build();

app.UseHttpLogging();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseAuthentication()
   .UseAuthorization();

app.UseFastEndpoints()
    .UseSwaggerGen();

app.Run();

public partial class Program { }
