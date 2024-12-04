using MediatR;
using Microsoft.Extensions.Logging;
using OrderDuplicate.Domain.Entities;
using OrderDuplicate.Domain.Events;

namespace OrderDuplicate.Application.Features.Group.EventHandlers;

public class GroupUpdatedEventHandler : INotificationHandler<UpdatedEvent<GroupEntity>>
{
    private readonly ILogger<GroupUpdatedEventHandler> _logger;

    public GroupUpdatedEventHandler(
        ILogger<GroupUpdatedEventHandler> logger
        )
    {
        _logger = logger;
    }
    public Task Handle(UpdatedEvent<GroupEntity> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().FullName);
        return Task.CompletedTask;
    }
}
