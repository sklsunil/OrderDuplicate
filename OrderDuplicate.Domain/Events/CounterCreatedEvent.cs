using OrderDuplicate.Domain.Common;
using OrderDuplicate.Domain.Entities;

namespace OrderDuplicate.Domain.Events;

public class CounterCreatedEvent : DomainEvent
{
    public CounterCreatedEvent(CounterEntity item)
    {
        Item = item;
    }

    public CounterEntity Item { get; }
}

