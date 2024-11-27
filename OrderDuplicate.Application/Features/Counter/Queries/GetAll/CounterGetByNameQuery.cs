using AutoMapper;

using MediatR;

using Microsoft.EntityFrameworkCore;

using OrderDuplicate.Application.Common;
using OrderDuplicate.Application.Common.Interfaces;
using OrderDuplicate.Application.Common.Interfaces.Caching;
using OrderDuplicate.Application.Features.Counter.DTOs;
using OrderDuplicate.Application.Features.Order.Caching;
using OrderDuplicate.Application.Features.Order.DTOs;

namespace OrderDuplicate.Application.Features.Counter.Queries.GetAll
{
    public class CounterGetByNameQuery : ICacheInvalidatorRequest<Result<CounterDto>>
    {
        public string Name { get; set; }
        public string CacheKey => OrderCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => OrderCacheKey.SharedExpiryTokenSource();

    }
    public class GetAllOrderItemsByOrderIdQueryHandler(IApplicationDbContext context,
   IMapper mapper
      ) :
           IRequestHandler<CounterGetByNameQuery, Result<CounterDto>>
    {
        private readonly IApplicationDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        public async Task<Result<CounterDto>> Handle(CounterGetByNameQuery request, CancellationToken cancellationToken)
        {
            var orderItems = await _context.Counters
                .FirstOrDefaultAsync(oli => oli.CounterName.Equals(request.Name), cancellationToken);            

            var orderItemDtos = _mapper.Map<CounterDto>(orderItems);
            return Result<CounterDto>.Success(orderItemDtos);
        }
    }
}
