
using OrderDuplicate.Domain.Entities;
using OrderDuplicate.Domain.Events;

using MediatR;

using Microsoft.Extensions.Logging;

namespace OrderDuplicate.Application.Features.Counter.EventHandlers;

public class OrderDeletedEventHandler(
    ILogger<OrderDeletedEventHandler> logger
        ) : INotificationHandler<DeletedEvent<OrderEntity>>
{
    private readonly ILogger<OrderDeletedEventHandler> _logger = logger;

    public Task Handle(DeletedEvent<OrderEntity> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().FullName);
        return Task.CompletedTask;
    }
}
