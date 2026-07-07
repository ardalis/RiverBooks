using Ardalis.Result;
using FluentValidation;
using FluentValidation.Results;
using Mediator;
using NSubstitute;
using Shouldly;
using Xunit;

namespace RiverBooks.SharedKernel.Tests;

public class FluentValidationBehavior_ValidationPasses
{
  [Fact]
  public async Task CallsNextAndReturnsItsResult()
  {
    // Arrange
    var request = CreateWidgetFixtures.MakeCommand(name: "Widget");

    var validator = Substitute.For<IValidator<CreateWidgetCommand>>();
    validator.ValidateAsync(Arg.Any<ValidationContext<CreateWidgetCommand>>(), Arg.Any<CancellationToken>())
      .Returns(new ValidationResult());

    var behavior = new FluentValidationBehavior<CreateWidgetCommand, Result<string>>(new[] { validator });

    var next = Substitute.For<MessageHandlerDelegate<CreateWidgetCommand, Result<string>>>();
    next(Arg.Any<CreateWidgetCommand>(), Arg.Any<CancellationToken>())
      .Returns(new ValueTask<Result<string>>(Result<string>.Success("widget-created")));

    // Act
    var result = await behavior.Handle(request, next, CancellationToken.None);

    // Assert
    _ = next.Received(1)(Arg.Any<CreateWidgetCommand>(), Arg.Any<CancellationToken>());
    result.Status.ShouldBe(ResultStatus.Ok);
    result.Value.ShouldBe("widget-created");
  }
}
