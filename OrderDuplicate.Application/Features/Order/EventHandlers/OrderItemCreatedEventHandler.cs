

using OrderDuplicate.Domain.Entities;
using OrderDuplicate.Domain.Events;

using MediatR;

using Microsoft.Extensions.Logging;

namespace OrderDuplicate.Application.Features.Order.EventHandlers;

public class OrderItemCreatedEventHandler(
    ILogger<OrderCreatedEventHandler> logger
        ) : INotificationHandler<CreatedEvent<OrderLineItemEntity>>
{
    private readonly ILogger<OrderCreatedEventHandler> _logger = logger;

    public Task Handle(CreatedEvent<OrderLineItemEntity> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().FullName);
        return Task.CompletedTask;
    }
}
