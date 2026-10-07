using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("RiverBooks.OrderProcessing.Tests")]
[assembly: InternalsVisibleTo("RiverBooks.OrderProcessingTests")]
// The Mediator source generator runs in the host and references handlers directly
[assembly: InternalsVisibleTo("RiverBooks.Web")]
// Allows NSubstitute (Castle DynamicProxy) to mock internal interfaces in tests
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace RiverBooks.OrderProcessing;

internal class AssemblyInfo { }
