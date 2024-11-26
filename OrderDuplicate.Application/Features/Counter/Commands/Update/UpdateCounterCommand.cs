using AutoMapper;

using OrderDuplicate.Application.Common;
using OrderDuplicate.Application.Common.Interfaces;
using OrderDuplicate.Application.Common.Interfaces.Caching;
using OrderDuplicate.Domain.Entities;
using OrderDuplicate.Domain.Events;

using MediatR;

using System.ComponentModel;
using OrderDuplicate.Application.Features.Counter.Caching;

namespace OrderDuplicate.Application.Features.Counter.Commands.Update;

public class UpdateCounterCommand : ICacheInvalidatorRequest<Result>
{
    [Description("Id")]
    public int Id { get; set; }

    [Description("User Id")]
    public int PersonId { get; set; }

    [Description("Counter Name")]
    public string CounterName { get; set; }

    public string CacheKey => CounterCacheKey.GetAllCacheKey;
    public CancellationTokenSource? SharedExpiryTokenSource => CounterCacheKey.SharedExpiryTokenSource();
}

public class UpdateCounterCommandHandler(
    IApplicationDbContext context,
     IMapper mapper
        ) : IRequestHandler<UpdateCounterCommand, Result>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<Result> Handle(UpdateCounterCommand request, CancellationToken cancellationToken)
    {
        var item = await _context.Counters.FindAsync(new object[] { request.Id }, cancellationToken);
        if (item != null)
        {
            item.CounterName = request.CounterName;
            item.PersonId = request.PersonId;
            // raise a update domain event
            item.AddDomainEvent(new UpdatedEvent<CounterEntity>(item));
            await _context.SaveChangesAsync(cancellationToken);
        }
        return await Result.SuccessAsync();
    }
}

