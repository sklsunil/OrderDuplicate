using OrderDuplicate.Domain.Common;

namespace OrderDuplicate.Domain.Events;
public class CreatedEvent<T> : DomainEvent where T : BaseEntity
{
    public CreatedEvent(T entity)
    {
        Entity = entity;
    }

    public T Entity { get; }
}
