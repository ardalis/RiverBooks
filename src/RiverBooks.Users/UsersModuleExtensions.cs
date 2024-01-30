using System.Text;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using RiverBooks.Users.Data;
using Serilog;

namespace RiverBooks.Users;

public static class UsersModuleExtensions
{
  public static IServiceCollection AddUserModuleServices(this IServiceCollection services,
    ConfigurationManager config,
    ILogger logger)
  {
    //services
    //  .AddAuthentication(
    //     o =>
    //     {
    //       o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    //       o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    //     })
    // .AddJwtBearer(
    //     o =>
    //     {
    //       o.TokenValidationParameters = new()
    //       {
    //         ValidateAudience = false,
    //         ValidateIssuer = false,
    //         ValidateLifetime = true,
    //         ValidateIssuerSigningKey = true,
    //         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Auth:JwtSecret"]!))
    //       };
    //     });

    string? connectionString = config.GetConnectionString("UsersConnectionString");
    services.AddDbContext<UsersDbContext>(config =>
      config.UseSqlServer(connectionString));

    services.AddIdentityCore<ApplicationUser>()
    .AddEntityFrameworkStores<UsersDbContext>()
    .AddApiEndpoints();

    logger.Information("{Module} module services registered", "Users");
    return services;
  }
}
