using FluentValidation;

namespace OrderDuplicate.Application.Features.Counter.Commands.Create;

public class CreateCounterCommandValidator : AbstractValidator<CreateCounterCommand>
{
    public CreateCounterCommandValidator()
    {
        RuleFor(v => v.CounterName)
             .MaximumLength(256)
             .NotEmpty();

    }
    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
 {
     var result = await ValidateAsync(ValidationContext<CreateCounterCommand>.CreateWithOptions((CreateCounterCommand)model, x => x.IncludeProperties(propertyName)));
     if (result.IsValid)
         return [];
     return result.Errors.Select(e => e.ErrorMessage);
 };
}

