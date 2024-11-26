using AutoMapper;
using AutoMapper.QueryableExtensions;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

using OrderDuplicate.Application.Common.Interfaces;
using OrderDuplicate.Application.Common.Interfaces.Caching;
using OrderDuplicate.Application.Common.Mappings;
using OrderDuplicate.Application.Common.Models;
using OrderDuplicate.Application.Features.Order.Caching;
using OrderDuplicate.Application.Features.Order.DTOs;
using OrderDuplicate.Application.Model;

using System.Linq.Dynamic.Core;

namespace OrderDuplicate.Application.Features.Order.Queries.Pagination;

public class OrderWithPaginationQuery : PaginationFilter, ICacheableRequest<PaginatedData<OrderDto>>
{
    public string CacheKey => OrderCacheKey.GetPaginationCacheKey($"{this}");
    public MemoryCacheEntryOptions? Options => OrderCacheKey.MemoryCacheEntryOptions;
}

public class OrderWithPaginationQueryHandler(
    IApplicationDbContext context,
    IMapper mapper
        ) :
     IRequestHandler<OrderWithPaginationQuery, PaginatedData<OrderDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<PaginatedData<OrderDto>> Handle(OrderWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var data = await _context
                            .Orders
                            .Include(x => x.Items)
                            .OrderBy($"{request.OrderBy} {request.SortDirection}")
                            .ProjectTo<OrderDto>(_mapper.ConfigurationProvider)
                            .PaginatedDataAsync(request.PageNumber, request.PageSize);
        return data;
    }
}