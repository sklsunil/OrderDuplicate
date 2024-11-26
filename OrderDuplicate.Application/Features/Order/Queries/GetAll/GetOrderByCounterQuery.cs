using AutoMapper;

using MediatR;

using Microsoft.EntityFrameworkCore;

using OrderDuplicate.Application.Common;
using OrderDuplicate.Application.Common.Exceptions;
using OrderDuplicate.Application.Common.Interfaces;
using OrderDuplicate.Application.Common.Interfaces.Caching;
using OrderDuplicate.Application.Features.Order.Caching;
using OrderDuplicate.Application.Features.Order.DTOs;

namespace OrderDuplicate.Application.Features.Order.Queries.GetAll
{
    public class GetOrderByCounterQuery(int counterId) : ICacheInvalidatorRequest<Result<List<OrderDto>>>
    {
        public int Id { get; } = counterId;
        public string CacheKey => OrderCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => OrderCacheKey.SharedExpiryTokenSource();
    }

    public class GetOrderByCounterCommandHandler(
    IApplicationDbContext context,
     IMapper mapper
        ) :
             IRequestHandler<GetOrderByCounterQuery, Result<List<OrderDto>>>

    {
        private readonly IApplicationDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        public async Task<Result<List<OrderDto>>> Handle(GetOrderByCounterQuery request, CancellationToken cancellationToken)
        {
            var counter = await _context
                                    .Counters
                                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken) ?? throw new NotFoundException($"Counters id: '{request.Id}' not found");
            
            var items = await _context
                                .Orders
                                .Include(x => x.Items)
                                .Where(x => request.Id == counter.PersonId).ToListAsync(cancellationToken);

            return await Result<List<OrderDto>>.SuccessAsync(_mapper.Map<List<OrderDto>>(items));
        }

    }
}
