using AutoMapper;

using MediatR;

using Microsoft.EntityFrameworkCore;

using OrderDuplicate.Application.Common;
using OrderDuplicate.Application.Common.Interfaces;
using OrderDuplicate.Application.Common.Interfaces.Caching;
using OrderDuplicate.Application.Features.Order.Caching;
using OrderDuplicate.Application.Features.Order.DTOs;

namespace OrderDuplicate.Application.Features.Order.Queries.GetAll
{
    public class GetAllOrderItemsByOrderIdQuery(int orderId) : ICacheInvalidatorRequest<Result<List<OrderItemDto>>>
    {
        public int Id { get; } = orderId;
        public string CacheKey => OrderCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => OrderCacheKey.SharedExpiryTokenSource();
    }
    public class GetAllOrderItemsByOrderIdQueryHandler(IApplicationDbContext context,
     IMapper mapper
        ) :
             IRequestHandler<GetAllOrderItemsByOrderIdQuery, Result<List<OrderItemDto>>>
    {
        private readonly IApplicationDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        public async Task<Result<List<OrderItemDto>>> Handle(GetAllOrderItemsByOrderIdQuery request, CancellationToken cancellationToken)
        {
            var orderItems = await _context.OrderItems
                .Where(oli => oli.OrderId == request.Id)
                .ToListAsync(cancellationToken);

            if (orderItems == null || orderItems.Count == 0)
            {
                return Result<List<OrderItemDto>>.Failure(["No order items found for the given order ID."]);
            }

            var orderItemDtos = _mapper.Map<List<OrderItemDto>>(orderItems);
            return Result<List<OrderItemDto>>.Success(orderItemDtos);
        }
    }
}
