using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using OrderDuplicate.Application.Common.Interfaces.Caching;
using OrderDuplicate.Application.Common.Interfaces;
using OrderDuplicate.Application.Features.Group.Dto;
using OrderDuplicate.Application.Features.Group.Caching;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace OrderDuplicate.Application.Features.Group.Queries.GetAll;

public class GetAllGroupQuery : ICacheableRequest<IEnumerable<GroupDto>>
{
    public string CacheKey => GroupCacheKey.GetAllCacheKey;
    public MemoryCacheEntryOptions? Options => GroupCacheKey.MemoryCacheEntryOptions;
}

public class GetAllGroupQueryHandler(
    IApplicationDbContext context,
    IMapper mapper
        ) :
     IRequestHandler<GetAllGroupQuery, IEnumerable<GroupDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<GroupDto>> Handle(GetAllGroupQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Groups
                     .ProjectTo<GroupDto>(_mapper.ConfigurationProvider)
                     .ToListAsync(cancellationToken);
        return data;
    }
}