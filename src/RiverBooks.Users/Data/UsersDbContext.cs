﻿using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RiverBooks.Users.Data;
internal class UsersDbContext : IdentityDbContext
{
  public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options)
  {
  }

  public DbSet<ApplicationUser> ApplicationUsers { get; set; } 

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.HasDefaultSchema("Users");

    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

    base.OnModelCreating(modelBuilder);
  }


}