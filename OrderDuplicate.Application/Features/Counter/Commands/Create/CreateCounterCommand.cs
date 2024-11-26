using AutoMapper;

using OrderDuplicate.Application.Common;
using OrderDuplicate.Application.Common.Interfaces;
using OrderDuplicate.Application.Common.Interfaces.Caching;
using OrderDuplicate.Application.Common.Mappings;
using OrderDuplicate.Domain.Entities;
using OrderDuplicate.Domain.Events;

using MediatR;

using System.ComponentModel;
using OrderDuplicate.Application.Features.Counter.DTOs;
using OrderDuplicate.Application.Features.Counter.Caching;

namespace OrderDuplicate.Application.Features.Counter.Commands.Create;

public class CreateCounterCommand : IMapFrom<CounterDto>, ICacheInvalidatorRequest<Result<int>>
{

    [Description("Counter Name")]
    public string CounterName { get; set; } 

    [Description("User Id")]
    public int PersonId { get; set; }

    public string CacheKey => CounterCacheKey.GetAllCacheKey;
    public CancellationTokenSource? SharedExpiryTokenSource => CounterCacheKey.SharedExpiryTokenSource();
}

public class CreateCounterCommandHandler(
    IApplicationDbContext context,
    IMapper mapper
        ) : IRequestHandler<CreateCounterCommand, Result<int>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<Result<int>> Handle(CreateCounterCommand request, CancellationToken cancellationToken)
    {
        var dto = _mapper.Map<CounterDto>(request);
        var item = _mapper.Map<CounterEntity>(dto);
        // raise a create domain event
        item.AddDomainEvent(new CreatedEvent<CounterEntity>(item));
        _context.Counters.Add(item);
        await _context.SaveChangesAsync(cancellationToken);
        return await Result<int>.SuccessAsync(item.Id);
    }
}

