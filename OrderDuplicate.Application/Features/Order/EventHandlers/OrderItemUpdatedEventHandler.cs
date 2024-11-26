using OrderDuplicate.Domain.Entities;
using OrderDuplicate.Domain.Events;

using MediatR;

using Microsoft.Extensions.Logging;

namespace OrderDuplicate.Application.Features.Order.EventHandlers;

public class OrderItemUpdatedEventHandler(
    ILogger<OrderItemUpdatedEventHandler> logger
        ) : INotificationHandler<UpdatedEvent<OrderLineItemEntity>>
{
    private readonly ILogger<OrderItemUpdatedEventHandler> _logger = logger;

    public Task Handle(UpdatedEvent<OrderLineItemEntity> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().FullName);
        return Task.CompletedTask;
    }
}
