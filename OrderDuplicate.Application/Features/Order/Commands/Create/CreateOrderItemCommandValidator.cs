using FluentValidation;

namespace OrderDuplicate.Application.Features.Order.Commands.Create;

public class CreateOrderItemCommandValidator : AbstractValidator<CreateOrderItemCommand>
{
    public CreateOrderItemCommandValidator()
    {

    }
    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
 {
     var result = await ValidateAsync(ValidationContext<CreateOrderItemCommand>.CreateWithOptions((CreateOrderItemCommand)model, x => x.IncludeProperties(propertyName)));
     if (result.IsValid)
         return [];
     return result.Errors.Select(e => e.ErrorMessage);
 };
}

