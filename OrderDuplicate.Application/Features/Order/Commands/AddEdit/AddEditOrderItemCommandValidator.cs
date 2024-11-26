using FluentValidation;

namespace OrderDuplicate.Application.Features.Order.Commands.AddEdit;

public class AddEditOrderItemCommandValidator : AbstractValidator<AddEditOrderItemCommand>
{
    public AddEditOrderItemCommandValidator()
    {
      
    }
    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<AddEditOrderItemCommand>.CreateWithOptions((AddEditOrderItemCommand)model, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
            return [];
        return result.Errors.Select(e => e.ErrorMessage);
    };
}

