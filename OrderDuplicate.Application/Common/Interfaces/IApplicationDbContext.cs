using OrderDuplicate.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;


namespace OrderDuplicate.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    ChangeTracker ChangeTracker { get; }
    DbSet<CounterEntity> Counters { get; set; }
    DbSet<OrderEntity> Orders { get; set; }
    DbSet<OrderLineItemEntity> OrderItems { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
