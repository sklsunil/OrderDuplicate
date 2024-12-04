using MediatR;
using Microsoft.Extensions.Logging;
using OrderDuplicate.Domain.Entities;
using OrderDuplicate.Domain.Events;

namespace OrderDuplicate.Application.Features.Group.EventHandlers;

public class GroupDeletedEventHandler : INotificationHandler<DeletedEvent<GroupEntity>>
{
    private readonly ILogger<GroupDeletedEventHandler> _logger;

    public GroupDeletedEventHandler(
        ILogger<GroupDeletedEventHandler> logger
        )
    {
        _logger = logger;
    }
    public Task Handle(DeletedEvent<GroupEntity> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().FullName);
        return Task.CompletedTask;
    }
}
