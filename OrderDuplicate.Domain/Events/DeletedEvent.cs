

using OrderDuplicate.Domain.Common;

namespace OrderDuplicate.Domain.Events;
public class DeletedEvent<T> : DomainEvent where T : BaseEntity
{
    public DeletedEvent(T entity)
    {
        Entity = entity;
    }

    public T Entity { get; }
}
