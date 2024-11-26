using OrderDuplicate.Domain.Common;
using OrderDuplicate.Domain.Entities;

namespace OrderDuplicate.Domain.Events;


public class CounterUpdatedEvent : DomainEvent
{
    public CounterUpdatedEvent(CounterEntity item)
    {
        Item = item;
    }

    public CounterEntity Item { get; }
}

