using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using Xunit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace RiverBooks.OrderProcessingTests.Arch;

public class SampleArchTest
{
  private static readonly Architecture Architecture =
    new ArchLoader()
      .LoadAssemblies(typeof(OrderProcessing.AssemblyInfo).Assembly)
      .Build();

  //[Fact]

}
