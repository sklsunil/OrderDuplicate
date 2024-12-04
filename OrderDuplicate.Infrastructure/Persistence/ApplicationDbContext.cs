using OrderDuplicate.Application.Common.Interfaces;
using OrderDuplicate.Domain.Common;
using OrderDuplicate.Domain.Entities;
using OrderDuplicate.Infrastructure.Persistence.Interceptors;

using System.Reflection;

namespace OrderDuplicate.Infrastructure.Persistence;
#nullable disable
public class ApplicationDbContext : DbContext, IApplicationDbContext
{


    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,

        AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor
        ) : base(options)
    {
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
    }

    public DbSet<CounterEntity> Counters { get; set; }
    public DbSet<OrderEntity> Orders { get; set; }
    public DbSet<OrderLineItemEntity> OrderItems { get; set; }
    public DbSet<GroupEntity> Groups { get; set; }
    public DbSet<GroupCounterEntity> GroupCounters { get; set; }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        builder.ApplyGlobalFilters<ISoftDelete>(s => s.Deleted == null);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditableEntitySaveChangesInterceptor);
    }
}
