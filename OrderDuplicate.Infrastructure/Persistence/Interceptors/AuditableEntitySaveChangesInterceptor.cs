using OrderDuplicate.Domain.Common;
using OrderDuplicate.Infrastructure.Extensions;

using MediatR;

using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace OrderDuplicate.Infrastructure.Persistence.Interceptors;


public class AuditableEntitySaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly IMediator _mediator;
    private List<DomainEvent> _deletingDomainEvents = new();
    public AuditableEntitySaveChangesInterceptor(
                IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {

        await UpdateEntities(eventData.Context);
        _deletingDomainEvents = await TryGetDeletingDomainEvents(eventData.Context, cancellationToken);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        var resultvalueTask = await base.SavedChangesAsync(eventData, result, cancellationToken);
        await _mediator.DispatchDomainEvents(eventData.Context!, _deletingDomainEvents);
        return resultvalueTask;
    }
    private Task UpdateEntities(DbContext? context)
    {
        string userId = null;
        foreach (var entry in context!.ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = userId;
                    entry.Entity.Created = DateTime.Now;
                    break;

                case EntityState.Modified:
                    entry.Entity.LastModifiedBy = userId;
                    entry.Entity.LastModified = DateTime.Now;
                    break;
                case EntityState.Deleted:
                    if (entry.Entity is ISoftDelete softDelete)
                    {
                        softDelete.DeletedBy = userId;
                        softDelete.Deleted = DateTime.Now;
                        entry.State = EntityState.Modified;
                    }
                    break;
                case EntityState.Unchanged:
                    if (entry.HasChangedOwnedEntities())
                    {
                        entry.Entity.LastModifiedBy = userId;
                        entry.Entity.LastModified = DateTime.Now;
                    }
                    break;
            }
        }
        return Task.CompletedTask;
    }
    private Task<List<DomainEvent>> TryGetDeletingDomainEvents(DbContext? context, CancellationToken cancellationToken = default)
    {
        if (context is null) return Task.FromResult(new List<DomainEvent>());
        var entities = context.ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Any() && e.State == EntityState.Deleted)
            .Select(e => e.Entity);

        var domainEvents = entities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        entities.ToList().ForEach(e => e.ClearDomainEvents());
        return Task.FromResult(domainEvents);
    }

}

public static class Extensions
{
    public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
        entry.References.Any(r =>
            r.TargetEntry != null &&
            r.TargetEntry.Metadata.IsOwned() &&
            (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
}