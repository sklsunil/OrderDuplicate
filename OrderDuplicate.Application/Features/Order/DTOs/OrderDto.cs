using AutoMapper;

using OrderDuplicate.Application.Common.Mappings;
using OrderDuplicate.Domain.Entities;

using System.ComponentModel;

namespace OrderDuplicate.Application.Features.Order.DTOs;


public class OrderDto : IMapFrom<OrderEntity>
{
    public void Mapping(Profile profile)
    {
        profile.CreateMap<OrderEntity, OrderDto>();
        profile.CreateMap<OrderDto, OrderEntity>();
    }

    [Description("Id")]
    public int Id { get; set; }

    [Description("Order Number")]
    public string OrderNumber { get; set; }
    public string CounterPersonId { get; set; }
    public bool IsCheckOut { get; set; }
    public List<OrderItemDto> Items { get; set; }
}
public class OrderItemDto : IMapFrom<OrderLineItemEntity>
{
    public void Mapping(Profile profile)
    {
        profile.CreateMap<OrderLineItemEntity, OrderItemDto>();
        profile.CreateMap<OrderItemDto, OrderLineItemEntity>();
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public int OrderId { get; set; }
}

