using Microsoft.EntityFrameworkCore.Metadata.Builders;

using OrderDuplicate.Domain.Entities;

namespace OrderDuplicate.Infrastructure.Persistence.Configurations
{
#nullable disable
    public class OrderConfiguration : IEntityTypeConfiguration<OrderEntity>
    {
        public void Configure(EntityTypeBuilder<OrderEntity> builder)
        {
            builder.Property(t => t.OrderNumber).HasMaxLength(255).IsRequired();
            builder.Property(t => t.CounterPersonId).IsRequired();
            builder.HasMany(t => t.Items)
                   .WithOne(i => i.Order)
                   .HasForeignKey(i => i.OrderId);
            builder.Ignore(e => e.DomainEvents);
        }
    }

}
