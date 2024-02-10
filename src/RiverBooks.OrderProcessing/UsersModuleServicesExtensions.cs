using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace RiverBooks.Users;

public static class OrderProcessingModuleServicesExtensions
{
  public static IServiceCollection AddOrderProcessingModuleServices(this IServiceCollection services,
    ConfigurationManager config,
    ILogger logger,
    List<System.Reflection.Assembly> mediatRAssemblies)
  {
    //string? connectionString = config.GetConnectionString("UsersConnectionString");
    //services.AddDbContext<UsersDbContext>(config =>
    //  config.UseSqlServer(connectionString));

    //services.AddScoped<IApplicationUserRepository, EfApplicationUserRepository>();

    // if using MediatR in this module, add any assemblies that contain handlers to the list
    mediatRAssemblies.Add(typeof(OrderProcessingModuleServicesExtensions).Assembly);

    logger.Information("{Module} module services registered", "Order Processing");
    return services;
  }
}
