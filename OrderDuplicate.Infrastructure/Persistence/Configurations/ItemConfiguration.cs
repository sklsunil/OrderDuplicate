using Microsoft.EntityFrameworkCore.Metadata.Builders;

using OrderDuplicate.Domain.Entities;

namespace OrderDuplicate.Infrastructure.Persistence.Configurations
{
    public class ItemConfiguration : IEntityTypeConfiguration<OrderLineItemEntity>
    {
        public void Configure(EntityTypeBuilder<OrderLineItemEntity> builder)
        {
            builder.Property(t => t.Name).HasMaxLength(255).IsRequired();
            builder.Property(t => t.Quantity).IsRequired();
            builder.Property(t => t.Price).IsRequired();
            builder.HasOne(t => t.Order)
                   .WithMany(o => o.Items)
                   .HasForeignKey(t => t.OrderId);
        }
    }
}
