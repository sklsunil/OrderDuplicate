using AutoMapper;
using AutoMapper.QueryableExtensions;

using MediatR;

using Microsoft.Extensions.Caching.Memory;

using OrderDuplicate.Application.Common.Interfaces;
using OrderDuplicate.Application.Common.Interfaces.Caching;
using OrderDuplicate.Application.Common.Mappings;
using OrderDuplicate.Application.Common.Models;
using OrderDuplicate.Application.Features.Counter.Caching;
using OrderDuplicate.Application.Features.Counter.DTOs;
using OrderDuplicate.Application.Model;

using System.Linq.Dynamic.Core;

namespace OrderDuplicate.Application.Features.Counter.Queries.Pagination;

public class CounterPaginationQuery : PaginationFilter, ICacheableRequest<PaginatedData<CounterDto>>
{
    public string CacheKey => CounterCacheKey.GetPaginationCacheKey($"{this}");
    public MemoryCacheEntryOptions? Options => CounterCacheKey.MemoryCacheEntryOptions;
}

public class CounterWithPaginationQueryHandler(
    IApplicationDbContext context,
    IMapper mapper
        ) :
     IRequestHandler<CounterPaginationQuery, PaginatedData<CounterDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<PaginatedData<CounterDto>> Handle(CounterPaginationQuery request, CancellationToken cancellationToken)
    {
        var data = _context.Counters.AsQueryable();
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
                else if (field.ToLower() == "countername" && adS.Keyword != null)
                {
                    data = data.Where(x => x.CounterName.Contains(adS.Keyword));
                }
                else if (field.ToLower() == "personId" && adS.Keyword != null)
                {
                    int key = Convert.ToInt32(adS.Keyword);
                    data = data.Where(x => x.PersonId == key);
                }
            }
        }

        var result = await data.OrderBy($"{request.OrderBy} {request.SortDirection}")
               .ProjectTo<CounterDto>(_mapper.ConfigurationProvider)
               .PaginatedDataAsync(request.PageNumber, request.PageSize);
        return result;
    }
}