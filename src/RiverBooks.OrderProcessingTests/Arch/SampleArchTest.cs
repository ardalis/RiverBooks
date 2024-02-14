using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnit;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Xunit;
using Xunit.Abstractions;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace RiverBooks.OrderProcessingTests.Arch;

public class InfrastructureDependencyTests
{
  public InfrastructureDependencyTests(ITestOutputHelper outputHelper)
  {
    _outputHelper = outputHelper;
  }
  private static readonly Architecture Architecture =
    new ArchLoader()
      .LoadAssemblies(typeof(OrderProcessing.AssemblyInfo).Assembly)
      .Build();
  private readonly ITestOutputHelper _outputHelper;

  [Fact]
  public void DomainClassesShouldNotReferenceInfrastructure()
  {
    var domainClasses = Classes().That()
      .ResideInNamespace("RiverBooks.OrderProcessing.Domain.*", useRegularExpressions : true)
      .As("OrderProcessing Domain Classes");

    var infrastructureClasses = Classes().That()
      .ResideInNamespace("RiverBooks.OrderProcessing.Infrastructure.*", useRegularExpressions: true)
      .As("Infrastructure Classes");

    var rule = domainClasses.Should().NotDependOnAny(infrastructureClasses);

    // Debugging - Inspect classes and their dependencies
    foreach (var domainClass in domainClasses.GetObjects(Architecture))
    {
      _outputHelper.WriteLine($"Domain Class: {domainClass.FullName}");
      foreach (var dependency in domainClass.Dependencies)
      {
        var targetClass = dependency.Target;
        if (infrastructureClasses.GetObjects(Architecture).Any(infraClass => infraClass.Equals(targetClass)))
        {
          _outputHelper.WriteLine($"  Depends on Infrastructure: {targetClass.FullName}");
        }
      }
    }

    foreach (var iClass in infrastructureClasses.GetObjects(Architecture))
    {
      _outputHelper.WriteLine($"Domain Class: {iClass.FullName}");
    }


    rule.Check(Architecture);
  }

}
