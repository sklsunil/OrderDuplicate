using OrderDuplicate.Domain.Entities;
using OrderDuplicate.Domain.Events;

using MediatR;

using Microsoft.Extensions.Logging;

namespace OrderDuplicate.Application.Features.Counter.EventHandlers;

public class OrderUpdatedEventHandler(
    ILogger<OrderUpdatedEventHandler> logger
        ) : INotificationHandler<UpdatedEvent<OrderEntity>>
{
    private readonly ILogger<OrderUpdatedEventHandler> _logger = logger;

    public Task Handle(UpdatedEvent<OrderEntity> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().FullName);
        return Task.CompletedTask;
    }
}
