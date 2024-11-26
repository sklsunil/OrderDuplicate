

using OrderDuplicate.Domain.Entities;
using OrderDuplicate.Domain.Events;

using MediatR;

using Microsoft.Extensions.Logging;

namespace OrderDuplicate.Application.Features.Order.EventHandlers;

public class OrderCreatedEventHandler : INotificationHandler<CreatedEvent<OrderEntity>>
{
    private readonly ILogger<OrderCreatedEventHandler> _logger;

    public OrderCreatedEventHandler(
        ILogger<OrderCreatedEventHandler> logger
        )
    {
        _logger = logger;
    }
    public Task Handle(CreatedEvent<OrderEntity> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().FullName);
        return Task.CompletedTask;
    }
}
