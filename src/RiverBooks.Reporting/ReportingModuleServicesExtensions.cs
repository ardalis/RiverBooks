using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RiverBooks.Reporting.Integrations;
using Serilog;

namespace RiverBooks.Reporting;

public static class ReportingModuleServicesExtensions
{
  public static IServiceCollection AddReportingModuleServices(this IServiceCollection services,
    ConfigurationManager config,
    ILogger logger,
    List<System.Reflection.Assembly> moduleAssemblies)
  {
    // configure module services
    services.AddScoped<ITopSellingBooksReportService, TopSellingBooksReportService>();
    services.AddScoped<ISalesReportService, DefaultSalesReportService>();
    services.AddScoped<OrderIngestionService>();

    // if using Mediator in this module, add any assemblies that contain handlers to the list
    moduleAssemblies.Add(typeof(ReportingModuleServicesExtensions).Assembly);

    logger.Information("{Module} module services registered", "Reporting");
    return services;
  }
}
