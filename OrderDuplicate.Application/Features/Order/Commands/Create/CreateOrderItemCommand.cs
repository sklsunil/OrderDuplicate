using AutoMapper;

using MediatR;

using OrderDuplicate.Application.Common;
using OrderDuplicate.Application.Common.Interfaces;
using OrderDuplicate.Application.Common.Interfaces.Caching;
using OrderDuplicate.Application.Common.Mappings;
using OrderDuplicate.Application.Features.Order.Caching;
using OrderDuplicate.Application.Features.Order.DTOs;
using OrderDuplicate.Domain.Entities;
using OrderDuplicate.Domain.Events;

namespace OrderDuplicate.Application.Features.Order.Commands.Create
{
    public class CreateOrderItemCommand : IMapFrom<OrderItemDto>, ICacheInvalidatorRequest<Result<int>>
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int OrderId { get; set; }
        public string CacheKey => OrderCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => OrderCacheKey.SharedExpiryTokenSource();
    }

    public class CreateOrderItemsCommand : IMapFrom<OrderItemDto>, ICacheInvalidatorRequest<Result<string>>
    {
        public List<CreateOrderItemCommand> Items { get; set; }
        public string CacheKey => OrderCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => OrderCacheKey.SharedExpiryTokenSource();
    }

    public class CreateOrderItemCommandHandler(
    IApplicationDbContext context,
    IMapper mapper
        ) : IRequestHandler<CreateOrderItemCommand, Result<int>>, IRequestHandler<CreateOrderItemsCommand, Result<string>>
    {
        private readonly IApplicationDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        public async Task<Result<int>> Handle(CreateOrderItemCommand request, CancellationToken cancellationToken)
        {
            var dto = _mapper.Map<OrderItemDto>(request);
            var item = _mapper.Map<OrderLineItemEntity>(dto);
            // raise a create domain event
            item.AddDomainEvent(new CreatedEvent<OrderLineItemEntity>(item));
            _context.OrderItems.Add(item);
            await _context.SaveChangesAsync(cancellationToken);
            return await Result<int>.SuccessAsync(item.Id);
        }

        public async Task<Result<string>> Handle(CreateOrderItemsCommand request, CancellationToken cancellationToken)
        {
            List<int> ids = new();
            foreach (var lineItems in request.Items)
            {
                var dto = _mapper.Map<OrderItemDto>(lineItems);
                var item = _mapper.Map<OrderLineItemEntity>(dto);
                // raise a create domain event
                item.AddDomainEvent(new CreatedEvent<OrderLineItemEntity>(item));
                _context.OrderItems.Add(item);
                ids.Add(item.Id);
            }
            await _context.SaveChangesAsync(cancellationToken);
            return await Result<string>.SuccessAsync("");
        }
    }
}
