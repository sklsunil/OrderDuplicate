using OrderDuplicate.Domain.Common;

namespace OrderDuplicate.Domain.Events;
public class UpdatedEvent<T> : DomainEvent where T : BaseEntity
{
    public UpdatedEvent(T entity)
    {
        Entity = entity;
    }

    public T Entity { get; }
}
