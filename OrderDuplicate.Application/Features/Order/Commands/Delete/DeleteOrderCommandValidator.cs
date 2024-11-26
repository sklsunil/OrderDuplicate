using FluentValidation;

namespace OrderDuplicate.Application.Features.Order.Commands.Delete;

public class DeleteOrderCommandValidator : AbstractValidator<DeleteOrderCommand>
{
    public DeleteOrderCommandValidator()
    {
        RuleFor(v => v.Id).NotNull().ForEach(v => v.GreaterThan(0));
    }
}


