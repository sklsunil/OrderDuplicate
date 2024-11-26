
using OrderDuplicate.Domain.Entities;
using OrderDuplicate.Domain.Events;

using MediatR;

using Microsoft.Extensions.Logging;

namespace OrderDuplicate.Application.Features.Counter.EventHandlers;

public class CounterDeletedEventHandler : INotificationHandler<DeletedEvent<CounterEntity>>
{
    private readonly ILogger<CounterDeletedEventHandler> _logger;

    public CounterDeletedEventHandler(
        ILogger<CounterDeletedEventHandler> logger
        )
    {
        _logger = logger;
    }
    public Task Handle(DeletedEvent<CounterEntity> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().FullName);
        return Task.CompletedTask;
    }
}
