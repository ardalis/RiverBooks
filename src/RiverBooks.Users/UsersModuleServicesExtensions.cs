﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RiverBooks.SharedKernel;
using RiverBooks.Users.Data;
using RiverBooks.Users.Domain;
using RiverBooks.Users.Interfaces;
using Serilog;

namespace RiverBooks.Users;

public static class UsersModuleServicesExtensions
{
  public static IServiceCollection AddUsersModuleServices(this IServiceCollection services,
    ConfigurationManager config,
    ILogger logger,
    List<System.Reflection.Assembly> mediatRAssemblies)
  {
    string? connectionString = config.GetConnectionString("UsersConnectionString");
    services.AddDbContext<UsersDbContext>(config =>
      config.UseSqlServer(connectionString));

    services.AddIdentityCore<ApplicationUser>()
            .AddEntityFrameworkStores<UsersDbContext>();

    services.AddScoped<IApplicationUserRepository, EfApplicationUserRepository>();
    services.AddScoped<IReadOnlyUserStreetAddressRepository, EfUserStreetAddressRepository>();

    // if using MediatR in this module, add any assemblies that contain handlers to the list
    mediatRAssemblies.Add(typeof(UsersModuleServicesExtensions).Assembly);

    logger.Information("{Module} module services registered", "Users");
    return services;
  }
}
