using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace RiverBooks.OrderProcessingTests.Arch;

// NOTE: NsDepCop is my preferred tool for enforcing architecture dependencies.
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
  public void DomainTypesShouldNotReferenceInfrastructure()
  {
    var domainTypes = GetTypesInNamespace("RiverBooks.OrderProcessing.Domain");
    var infrastructureTypes = GetTypesInNamespace("RiverBooks.OrderProcessing.Infrastructure");

    PrintTypes(domainTypes, infrastructureTypes);

    domainTypes.ShouldNotBeEmpty();
    infrastructureTypes.ShouldNotBeEmpty();

    FindInfrastructureDependencies(domainTypes, infrastructureTypes).ShouldBeEmpty();
  }

  [Fact]
  public void UseCaseTypesShouldNotReferenceInfrastructure()
  {
    var useCasesTypes = GetTypesInNamespace("RiverBooks.OrderProcessing.UseCases");
    var infrastructureTypes = GetTypesInNamespace("RiverBooks.OrderProcessing.Infrastructure");

    useCasesTypes.ShouldNotBeEmpty();
    infrastructureTypes.ShouldNotBeEmpty();

    FindInfrastructureDependencies(useCasesTypes, infrastructureTypes).ShouldBeEmpty();
  }

  private static List<IType> GetTypesInNamespace(string namespacePrefix)
  {
    return Architecture.Types
      .Where(type => type.FullName.StartsWith(namespacePrefix, StringComparison.Ordinal))
      .ToList();
  }

  private static List<string> FindInfrastructureDependencies(IEnumerable<IType> sourceTypes, IEnumerable<IType> infrastructureTypes)
  {
    var infrastructureNames = infrastructureTypes
      .Select(type => type.FullName)
      .ToHashSet(StringComparer.Ordinal);

    return sourceTypes
      .SelectMany(type => type.Dependencies
        .Where(dependency => infrastructureNames.Contains(dependency.Target.FullName))
        .Select(dependency => $"{type.FullName} -> {dependency.Target.FullName}"))
      .ToList();
  }

  /// <summary>
  /// Used for debugging purposes
  /// </summary>
  /// <param name="domainTypes"></param>
  /// <param name="infrastructureTypes"></param>
  private void PrintTypes(IEnumerable<IType> domainTypes, IEnumerable<IType> infrastructureTypes)
  {
    var infrastructureNames = infrastructureTypes
      .Select(type => type.FullName)
      .ToHashSet(StringComparer.Ordinal);

    // Debugging - Inspect classes and their dependencies
    foreach (var domainClass in domainTypes)
    {
      _outputHelper.WriteLine($"Domain Type: {domainClass.FullName}");
      foreach (var dependency in domainClass.Dependencies)
      {
        var targetType = dependency.Target;
        if (infrastructureNames.Contains(targetType.FullName))
        {
          _outputHelper.WriteLine($"  Depends on Infrastructure: {targetType.FullName}");
        }
      }
    }

    foreach (var iType in infrastructureTypes)
    {
      _outputHelper.WriteLine($"Infrastructure Type: {iType.FullName}");
    }
  }
}
