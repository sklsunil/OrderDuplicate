using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using OrderDuplicate.Application.Common.Interfaces.Caching;
using OrderDuplicate.Application.Common.Interfaces;
using OrderDuplicate.Application.Common.Models;
using OrderDuplicate.Application.Model;
using OrderDuplicate.Application.Features.Group.Dto;
using OrderDuplicate.Application.Features.Group.Caching;
using System.Linq.Dynamic.Core;
using AutoMapper.QueryableExtensions;
using OrderDuplicate.Application.Common.Mappings;

namespace OrderDuplicate.Application.Features.Group.Queries.Pagination;

public class GroupWithPaginationQuery : PaginationFilter, ICacheableRequest<PaginatedData<GroupDto>>
{
    public string CacheKey => GroupCacheKey.GetPaginationCacheKey($"{this}");
    public MemoryCacheEntryOptions? Options => GroupCacheKey.MemoryCacheEntryOptions;
}

public class GroupWithPaginationQueryHandler(
    IApplicationDbContext context,
    IMapper mapper
        ) :
     IRequestHandler<GroupWithPaginationQuery, PaginatedData<GroupDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<PaginatedData<GroupDto>> Handle(GroupWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var data = await _context
                            .Groups
                            .OrderBy($"{request.OrderBy} {request.SortDirection}")
                            .ProjectTo<GroupDto>(_mapper.ConfigurationProvider)
                            .PaginatedDataAsync(request.PageNumber, request.PageSize);
        return data;
    }
}
