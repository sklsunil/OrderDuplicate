using AutoMapper;
using AutoMapper.QueryableExtensions;

using OrderDuplicate.Application.Common.Interfaces;
using OrderDuplicate.Application.Common.Interfaces.Caching;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using OrderDuplicate.Application.Features.Counter.DTOs;
using OrderDuplicate.Application.Features.Counter.Caching;

namespace OrderDuplicate.Application.Features.Counter.Queries.GetAll;

public class GetAllCounterQuery : ICacheableRequest<IEnumerable<CounterDto>>
{
    public string CacheKey => CounterCacheKey.GetAllCacheKey;
    public MemoryCacheEntryOptions? Options => CounterCacheKey.MemoryCacheEntryOptions;
}

public class GetAllCounterQueryHandler(
    IApplicationDbContext context,
    IMapper mapper
        ) :
     IRequestHandler<GetAllCounterQuery, IEnumerable<CounterDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<CounterDto>> Handle(GetAllCounterQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Counters
                     .ProjectTo<CounterDto>(_mapper.ConfigurationProvider)
                     .ToListAsync(cancellationToken);
        return data;
    }
}


