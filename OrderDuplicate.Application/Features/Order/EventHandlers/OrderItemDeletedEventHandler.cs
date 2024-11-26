
using OrderDuplicate.Domain.Entities;
using OrderDuplicate.Domain.Events;

using MediatR;

using Microsoft.Extensions.Logging;

namespace OrderDuplicate.Application.Features.Order.EventHandlers;

public class OrderItemDeletedEventHandler(
    ILogger<OrderItemDeletedEventHandler> logger
        ) : INotificationHandler<DeletedEvent<OrderLineItemEntity>>
{
    private readonly ILogger<OrderItemDeletedEventHandler> _logger = logger;

    public Task Handle(DeletedEvent<OrderLineItemEntity> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().FullName);
        return Task.CompletedTask;
    }
}
