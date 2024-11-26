using FluentValidation;

namespace OrderDuplicate.Application.Features.Order.Commands.AddEdit;

public class AddEditOrderCommandValidator : AbstractValidator<AddEditOrderCommand>
{
    public AddEditOrderCommandValidator()
    {
      
    }
    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<AddEditOrderCommand>.CreateWithOptions((AddEditOrderCommand)model, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
            return [];
        return result.Errors.Select(e => e.ErrorMessage);
    };
}

