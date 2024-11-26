

using OrderDuplicate.Domain.Entities;
using OrderDuplicate.Domain.Events;

using MediatR;

using Microsoft.Extensions.Logging;

namespace OrderDuplicate.Application.Features.Counter.EventHandlers;

public class CounterCreatedEventHandler : INotificationHandler<CreatedEvent<CounterEntity>>
{
    private readonly ILogger<CounterCreatedEventHandler> _logger;

    public CounterCreatedEventHandler(
        ILogger<CounterCreatedEventHandler> logger
        )
    {
        _logger = logger;
    }
    public Task Handle(CreatedEvent<CounterEntity> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().FullName);
        return Task.CompletedTask;
    }
}
