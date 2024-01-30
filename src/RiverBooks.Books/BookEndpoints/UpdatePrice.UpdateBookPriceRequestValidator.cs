using FastEndpoints;
using FluentValidation;

namespace RiverBooks.Books.BookEndpoints;
public class UpdateBookPriceRequestValidator : Validator<UpdateBookPriceRequest>
{
  public UpdateBookPriceRequestValidator()
  {
    RuleFor(x => x.Id)
      .NotNull()
      .NotEqual(Guid.Empty)
      .WithMessage("A book id is required.");

    RuleFor(x => x.NewPrice)
        .GreaterThanOrEqualTo(0m)
        .WithMessage("Book prices must be positive values.");
  }
}
