using OrderDuplicate.Domain.Common;
using OrderDuplicate.Domain.Entities;

namespace OrderDuplicate.Domain.Events;

public class CounterDeletedEvent : DomainEvent
{
    public CounterDeletedEvent(CounterEntity item)
    {
        Item = item;
    }

    public CounterEntity Item { get; }
}

