using FluentValidation;

namespace OrderDuplicate.Application.Features.Group.Commands.AddEdit;

public class AddEditGroupCommandValidator : AbstractValidator<AddEditGroupCommand>
{
    public AddEditGroupCommandValidator()
    {

    }
    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<AddEditGroupCommand>.CreateWithOptions((AddEditGroupCommand)model, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
            return [];
        return result.Errors.Select(e => e.ErrorMessage);
    };
}


