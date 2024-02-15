using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using RiverBooks.EmailSending.SendQueuedEmail;
using Serilog;

namespace RiverBooks.EmailSending;

public static class EmailSendingModuleServicesExtensions
{
  public static IServiceCollection AddEmailSendingModuleServices(this IServiceCollection services,
    ConfigurationManager config,
    ILogger logger,
    List<System.Reflection.Assembly> mediatRAssemblies)
  {
    // configure MongoDB
    services.Configure<MongoDBSettings>(config.GetSection("MongoDB"));
    services.AddMongoDB(config);

    // configure module services
    services.AddSingleton<IOutboxProcessor, MongoDbEmailOutboxProcessor>();
    services.AddTransient<ISendEmail, MimeKitEmailSender>();

    // if using MediatR in this module, add any assemblies that contain handlers to the list
    mediatRAssemblies.Add(typeof(EmailSendingModuleServicesExtensions).Assembly);

    // Add BackgroundWorker
    services.AddHostedService<EmailSendingBackgroundService>();

    logger.Information("{Module} module services registered", "Email Sending");
    return services;
  }

  public static IServiceCollection AddMongoDB(this IServiceCollection services, IConfiguration configuration)
  {
    // Register the MongoDB client as a singleton
    services.AddSingleton<IMongoClient>(serviceProvider =>
    {
      var settings = configuration.GetSection("MongoDB").Get<MongoDBSettings>();
      return new MongoClient(settings!.ConnectionString);
    });

    // Register the MongoDB database as a singleton
    services.AddSingleton(serviceProvider =>
    {
      var settings = configuration.GetSection("MongoDB").Get<MongoDBSettings>();
      var client = serviceProvider.GetService<IMongoClient>();
      return client!.GetDatabase(settings!.DatabaseName);
    });

    //// Optionally, register specific collections here as scoped or singleton services
    //// Example for a 'EmailOutboxEntity' collection
    services.AddTransient(serviceProvider =>
    {
      var database = serviceProvider.GetService<IMongoDatabase>();
      return database!.GetCollection<EmailOutboxEntity>("EmailOutboxEntityCollection");
    });

    return services;
  }
}
