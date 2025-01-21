using Microsoft.EntityFrameworkCore.ChangeTracking;


namespace OrderDuplicate.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    ChangeTracker ChangeTracker { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
