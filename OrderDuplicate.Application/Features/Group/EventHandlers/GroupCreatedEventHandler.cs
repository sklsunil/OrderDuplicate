using MediatR;
using Microsoft.Extensions.Logging;
using OrderDuplicate.Domain.Entities;
using OrderDuplicate.Domain.Events;

namespace OrderDuplicate.Application.Features.Group.EventHandlers;

public class GroupCreatedEventHandler : INotificationHandler<CreatedEvent<GroupEntity>>
{
    private readonly ILogger<GroupCreatedEventHandler> _logger;

    public GroupCreatedEventHandler(
        ILogger<GroupCreatedEventHandler> logger
        )
    {
        _logger = logger;
    }
    public Task Handle(CreatedEvent<GroupEntity> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().FullName);
        return Task.CompletedTask;
    }
}
