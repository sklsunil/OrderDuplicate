using FluentValidation;

using OrderDuplicate.Application.Features.Counter.Commands.Update;

namespace OrderDuplicate.Application.Features.Counter.Commands.Update;

public class UpdateCounterCommandValidator : AbstractValidator<UpdateCounterCommand>
{
    public UpdateCounterCommandValidator()
    {
        RuleFor(v => v.CounterName)
           .MaximumLength(256)
           .NotEmpty();
       
    }
    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<UpdateCounterCommand>.CreateWithOptions((UpdateCounterCommand)model, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
            return [];
        return result.Errors.Select(e => e.ErrorMessage);
    };
}

