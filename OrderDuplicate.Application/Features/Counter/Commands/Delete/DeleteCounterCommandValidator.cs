using FluentValidation;

using OrderDuplicate.Application.Features.Counter.Commands.Delete;

namespace OrderDuplicate.Application.Features.Counter.Commands.Delete;

public class DeleteCounterCommandValidator : AbstractValidator<DeleteCounterCommand>
{
    public DeleteCounterCommandValidator()
    {
        RuleFor(v => v.Id).NotNull().ForEach(v => v.GreaterThan(0));
    }
}


