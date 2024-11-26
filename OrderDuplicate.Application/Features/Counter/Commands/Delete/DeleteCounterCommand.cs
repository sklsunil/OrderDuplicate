using AutoMapper;

using OrderDuplicate.Application.Common;
using OrderDuplicate.Application.Common.Interfaces;
using OrderDuplicate.Application.Common.Interfaces.Caching;
using OrderDuplicate.Application.Features.Counter.Caching;
using OrderDuplicate.Domain.Entities;
using OrderDuplicate.Domain.Events;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace OrderDuplicate.Application.Features.Counter.Commands.Delete;

public class DeleteCounterCommand(List<int> id) : ICacheInvalidatorRequest<Result>
{
    public List<int> Id { get; } = id;
    public string CacheKey => CounterCacheKey.GetAllCacheKey;
    public CancellationTokenSource? SharedExpiryTokenSource => CounterCacheKey.SharedExpiryTokenSource();
}

public class DeleteCounterCommandHandler(
    IApplicationDbContext context,
     IMapper mapper
        ) :
             IRequestHandler<DeleteCounterCommand, Result>

{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<Result> Handle(DeleteCounterCommand request, CancellationToken cancellationToken)
    {
        var items = await _context.Counters.Where(x => request.Id.Contains(x.Id)).ToListAsync(cancellationToken);
        foreach (var item in items)
        {
            // raise a delete domain event
            item.AddDomainEvent(new DeletedEvent<CounterEntity>(item));
            _context.Counters.Remove(item);
        }
        await _context.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync();
    }

}

