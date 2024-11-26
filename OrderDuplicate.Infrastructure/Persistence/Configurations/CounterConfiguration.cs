using OrderDuplicate.Domain.Entities;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OrderDuplicate.Infrastructure.Persistence.Configurations;

#nullable disable
public class CounterConfiguration : IEntityTypeConfiguration<CounterEntity>
{
    public void Configure(EntityTypeBuilder<CounterEntity> builder)
    {         
        builder.Property(t => t.PersonId).IsRequired();
        builder.Property(t => t.CounterName).HasMaxLength(255).IsRequired();
        builder.Ignore(e => e.DomainEvents);
    }
}


