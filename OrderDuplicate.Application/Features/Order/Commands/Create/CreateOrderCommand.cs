using AutoMapper;

using MediatR;

using OrderDuplicate.Application.Common;
using OrderDuplicate.Application.Common.Interfaces;
using OrderDuplicate.Application.Common.Interfaces.Caching;
using OrderDuplicate.Application.Common.Mappings;
using OrderDuplicate.Application.Features.Order.DTOs;
using OrderDuplicate.Application.Features.Order.Caching;
using OrderDuplicate.Domain.Entities;
using OrderDuplicate.Domain.Events;

namespace OrderDuplicate.Application.Features.Order.Commands.Create;

public class CreateOrderCommand : IMapFrom<OrderDto>, ICacheInvalidatorRequest<Result<int>>
{
    public string OrderNumber { get; set; }
    public string CounterPersonId { get; set; }
    public bool IsCheckOut { get; set; }
    public string CacheKey => OrderCacheKey.GetAllCacheKey;
    public CancellationTokenSource? SharedExpiryTokenSource => OrderCacheKey.SharedExpiryTokenSource();
}

public class CreateOrderCommandHandler(
    IApplicationDbContext context,
    IMapper mapper
        ) : IRequestHandler<CreateOrderCommand, Result<int>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<Result<int>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var dto = _mapper.Map<OrderDto>(request);
        var item = _mapper.Map<OrderEntity>(dto);
        // raise a create domain event
        item.AddDomainEvent(new CreatedEvent<OrderEntity>(item));
        _context.Orders.Add(item);
        await _context.SaveChangesAsync(cancellationToken);
        return await Result<int>.SuccessAsync(item.Id);
    }
}

