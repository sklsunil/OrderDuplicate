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
using OrderDuplicate.Application.Features.Counter.DTOs;

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
        var data = _context.Groups.AsQueryable();

        var adS = request.AdvancedSearch;
        if (adS != null)
        {
            foreach (var field in adS.Fields)
            {
                if (field.ToLower() == "id" && adS.Keyword != null)
                {
                    var isDigit = adS.Keyword.All(char.IsDigit);
                    if (isDigit)
                    {
                        var id = int.Parse(adS.Keyword);
                        data = data.Where(x => x.Id == id);
                    }
                }
                else if (field.ToLower() == "groupname" && adS.Keyword != null)
                {
                    data = data.Where(x => x.GroupName.Contains(adS.Keyword));
                }
                else if (field.ToLower() == "countername" && adS.Keyword != null)
                {
                    data = data.Where(x => x.GroupCounters.Any(gc => gc.Counter.CounterName.Contains(adS.Keyword)));
                }
            }
        }

        var result = await data.OrderBy($"{request.OrderBy} {request.SortDirection}")
               .ProjectTo<GroupDto>(_mapper.ConfigurationProvider)
               .PaginatedDataAsync(request.PageNumber, request.PageSize);

        return result;
    }
}
