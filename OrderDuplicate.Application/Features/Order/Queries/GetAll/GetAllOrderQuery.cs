using AutoMapper;
using AutoMapper.QueryableExtensions;

using OrderDuplicate.Application.Common.Interfaces;
using OrderDuplicate.Application.Common.Interfaces.Caching;
using OrderDuplicate.Application.Features.Order.DTOs;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using OrderDuplicate.Application.Features.Order.Caching;

namespace OrderDuplicate.Application.Features.Order.Queries.GetAll;

public class GetAllOrderQuery : ICacheableRequest<IEnumerable<OrderDto>>
{
    public string CacheKey => OrderCacheKey.GetAllCacheKey;
    public MemoryCacheEntryOptions? Options => OrderCacheKey.MemoryCacheEntryOptions;
}

public class GetAllOrderQueryHandler(
    IApplicationDbContext context,
    IMapper mapper
        ) :
     IRequestHandler<GetAllOrderQuery, IEnumerable<OrderDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<OrderDto>> Handle(GetAllOrderQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Orders
                     .ProjectTo<OrderDto>(_mapper.ConfigurationProvider)
                     .ToListAsync(cancellationToken);
        return data;
    }
}


