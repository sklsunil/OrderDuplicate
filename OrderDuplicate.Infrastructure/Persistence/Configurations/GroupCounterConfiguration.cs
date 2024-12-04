using Microsoft.EntityFrameworkCore.Metadata.Builders;

using OrderDuplicate.Domain.Entities;

namespace OrderDuplicate.Infrastructure.Persistence.Configurations
{
    public class GroupCounterEntityConfiguration : IEntityTypeConfiguration<GroupCounterEntity>
    {
        public void Configure(EntityTypeBuilder<GroupCounterEntity> builder)
        {
            builder.HasKey(gc => new { gc.GroupId, gc.CounterId });

            builder.HasOne(gc => gc.Group)
                .WithMany(g => g.GroupCounters)
                .HasForeignKey(gc => gc.GroupId);

            builder.HasOne(gc => gc.Counter)
                .WithMany(c => c.GroupCounters)
                .HasForeignKey(gc => gc.CounterId);
        }
    }
}
