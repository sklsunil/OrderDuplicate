using AutoMapper;
using MediatR;
using OrderDuplicate.Application.Common.Exceptions;
using OrderDuplicate.Application.Common.Interfaces.Caching;
using OrderDuplicate.Application.Common.Interfaces;
using OrderDuplicate.Application.Features.Group.Dto;
using OrderDuplicate.Application.Common;
using OrderDuplicate.Application.Features.Group.Caching;
using Microsoft.EntityFrameworkCore;

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
        var counter = await _context
                                .Counters
                                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken) ?? throw new NotFoundException($"Counters id: '{request.Id}' not found");

        var items = await _context
                            .Groups
                            .Where(x => request.Id == counter.PersonId).ToListAsync(cancellationToken);

        return await Result<List<GroupDto>>.SuccessAsync(_mapper.Map<List<GroupDto>>(items));
    }

}

