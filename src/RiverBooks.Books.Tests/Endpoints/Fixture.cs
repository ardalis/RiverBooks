using FastEndpoints.Testing;
using Xunit.Abstractions;

namespace RiverBooks.Books.Tests.Endpoints;

public class Fixture(IMessageSink messageSink) 
  : AppFixture<Program>(messageSink)
{
}
