using Microsoft.EntityFrameworkCore.Migrations;
using RiverBooks.Users;
using Shouldly;
using Xunit;

namespace RiverBooks.OrderProcessingTests.Arch;

// Other modules should interact with OrderProcessing only through its Contracts project.
// The module itself exposes nothing but its service registration entry point.
public class PublicApiSurfaceTests
{
  [Fact]
  public void OnlyModuleServicesExtensionsIsPublic()
  {
    var moduleAssembly = typeof(OrderProcessingModuleServicesExtensions).Assembly;

    // EF Core tooling generates migrations as public; they aren't part of the module's API
    var publicTypes = moduleAssembly.GetExportedTypes()
      .Where(type => !typeof(Migration).IsAssignableFrom(type))
      .Select(type => type.FullName)
      .ToList();

    publicTypes.ShouldBe([typeof(OrderProcessingModuleServicesExtensions).FullName]);
  }
}
