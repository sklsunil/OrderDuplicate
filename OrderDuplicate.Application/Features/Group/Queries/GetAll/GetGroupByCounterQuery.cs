using AutoMapper;

using MediatR;

using Microsoft.EntityFrameworkCore;

using OrderDuplicate.Application.Common;
using OrderDuplicate.Application.Common.Interfaces;
using OrderDuplicate.Application.Common.Interfaces.Caching;
using OrderDuplicate.Application.Features.Group.Caching;
using OrderDuplicate.Application.Features.Group.Dto;

namespace OrderDuplicate.Application.Features.Group.Queries.GetAll;

public class GetGroupByCounterQuery() : ICacheInvalidatorRequest<Result<List<GroupDto>>>
{
    public int Id { get; set; }
    public string CacheKey => GroupCacheKey.GetAllCacheKey;
    public CancellationTokenSource? SharedExpiryTokenSource => GroupCacheKey.SharedExpiryTokenSource();

}

public class GetGroupByCounterCommandHandler(
IApplicationDbContext context,
 IMapper mapper
    ) :
         IRequestHandler<GetGroupByCounterQuery, Result<List<GroupDto>>>

{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<Result<List<GroupDto>>> Handle(GetGroupByCounterQuery request, CancellationToken cancellationToken)
    {
        var items = await _context
                            .Groups.Include(x => x.GroupCounters).ThenInclude(x => x.Counter)
                            .Where(x => x.GroupCounters.Any(g => g.CounterId == request.Id))
                            .ToListAsync(cancellationToken);

        return await Result<List<GroupDto>>.SuccessAsync(_mapper.Map<List<GroupDto>>(items));
    }

}

