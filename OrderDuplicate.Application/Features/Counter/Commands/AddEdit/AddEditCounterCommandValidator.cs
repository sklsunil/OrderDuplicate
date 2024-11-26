using FluentValidation;

namespace OrderDuplicate.Application.Features.Counter.Commands.AddEdit;

public class AddEditCounterCommandValidator : AbstractValidator<AddEditCounterCommand>
{
    public AddEditCounterCommandValidator()
    {
        RuleFor(v => v.CounterName)
            .MaximumLength(256)
            .NotEmpty();
    }
    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<AddEditCounterCommand>.CreateWithOptions((AddEditCounterCommand)model, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
            return [];

        return result.Errors.Select(e => e.ErrorMessage);
    };
}

