using OrderDuplicate.Domain.Entities;
using OrderDuplicate.Domain.Events;

using MediatR;

using Microsoft.Extensions.Logging;

namespace OrderDuplicate.Application.Features.Counter.EventHandlers;

public class CounterUpdatedEventHandler : INotificationHandler<UpdatedEvent<CounterEntity>>
{
    private readonly ILogger<CounterUpdatedEventHandler> _logger;

    public CounterUpdatedEventHandler(
        ILogger<CounterUpdatedEventHandler> logger
        )
    {
        _logger = logger;
    }
    public Task Handle(UpdatedEvent<CounterEntity> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().FullName);
        return Task.CompletedTask;
    }
}
