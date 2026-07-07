using System.Runtime.CompilerServices;
using Ardalis.Result;
using FluentValidation;
using FluentValidation.Results;
using Mediator;
using NSubstitute;
using Shouldly;
using Xunit;

// FluentValidation is strong-named, so NSubstitute's Castle-based proxy generator needs an
// explicit grant to create a substitute for IValidator<CreateWidgetCommand> when
// CreateWidgetCommand is `internal` rather than `public`.
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c547cac37abd99c8db225ef2f6c8a3602f3b3606cc9891605d02baa56104f4cfc0734aa39b93bf7852f7d9266654753cc297e7d2edfe0bac1cdcf9f717241550e0a7b191195b7667bb4f64bcb8e2121380fd1d9d46ad2d92d2d15605093924cceaf74c4861eff62abf69b9291ed0a340e113be11e6a7d3113e92484cf7045cc7")]

namespace RiverBooks.SharedKernel.Tests;

public class FluentValidationBehavior_ValidationFails
{
  [Fact]
  public async Task ReturnsInvalidResultWithErrors()
  {
    // Arrange
    var request = CreateWidgetFixtures.MakeCommand(name: "");

    var validator = Substitute.For<IValidator<CreateWidgetCommand>>();
    validator.ValidateAsync(Arg.Any<ValidationContext<CreateWidgetCommand>>(), Arg.Any<CancellationToken>())
      .Returns(new ValidationResult(new[]
      {
        new ValidationFailure("Name", "Name is required.")
      }));

    var behavior = new FluentValidationBehavior<CreateWidgetCommand, Result<string>>(new[] { validator });

    var next = Substitute.For<MessageHandlerDelegate<CreateWidgetCommand, Result<string>>>();
    next(Arg.Any<CreateWidgetCommand>(), Arg.Any<CancellationToken>())
      .Returns(new ValueTask<Result<string>>(Result<string>.Success("should-not-be-reached")));

    // Act
    var result = await behavior.Handle(request, next, CancellationToken.None);

    // Assert
    result.Status.ShouldBe(ResultStatus.Invalid);
    result.ValidationErrors.ShouldContain(e => e.ErrorMessage == "Name is required.");
    _ = next.DidNotReceive()(Arg.Any<CreateWidgetCommand>(), Arg.Any<CancellationToken>());
  }
}

internal record CreateWidgetCommand(string Name) : IRequest<Result<string>>;

internal static class CreateWidgetFixtures
{
  public static CreateWidgetCommand MakeCommand(string name = "Widget") => new(name);
}
