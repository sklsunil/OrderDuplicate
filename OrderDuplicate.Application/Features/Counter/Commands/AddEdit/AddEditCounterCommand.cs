using AutoMapper;

using OrderDuplicate.Application.Common;
using OrderDuplicate.Application.Common.Exceptions;
using OrderDuplicate.Application.Common.Interfaces;
using OrderDuplicate.Application.Common.Interfaces.Caching;
using OrderDuplicate.Application.Common.Mappings;
using OrderDuplicate.Domain.Entities;
using OrderDuplicate.Domain.Events;

using MediatR;

using System.ComponentModel;
using OrderDuplicate.Application.Features.Counter.DTOs;
using OrderDuplicate.Application.Features.Counter.Caching;

namespace OrderDuplicate.Application.Features.Counter.Commands.AddEdit;

public class AddEditCounterCommand : IMapFrom<CounterDto>, ICacheInvalidatorRequest<Result<int>>
{
    [Description("Id")]
    public int Id { get; set; }

    [Description("Counter Name")]
    public string CounterName { get; set; }

    [Description("User Id")]
    public int UserId { get; set; }
    public string CacheKey => CounterCacheKey.GetAllCacheKey;
    public CancellationTokenSource? SharedExpiryTokenSource => CounterCacheKey.SharedExpiryTokenSource();
}

public class AddEditCounterCommandHandler(
    IApplicationDbContext context,
    IMapper mapper
        ) : IRequestHandler<AddEditCounterCommand, Result<int>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<Result<int>> Handle(AddEditCounterCommand request, CancellationToken cancellationToken)
    {
        var dto = _mapper.Map<CounterDto>(request);
        if (request.Id > 0)
        {
            var item = await _context.Counters.FindAsync(new object[] { request.Id }, cancellationToken) ?? throw new NotFoundException($"The Counter [{request.Id}] was not found.");
            item = _mapper.Map(dto, item);
            // raise a update domain event
            item.AddDomainEvent(new UpdatedEvent<CounterEntity>(item));
            await _context.SaveChangesAsync(cancellationToken);
            return await Result<int>.SuccessAsync(item.Id);
        }
        else
        {
            var item = _mapper.Map<CounterEntity>(dto);
            // raise a create domain event
            item.AddDomainEvent(new CreatedEvent<CounterEntity>(item));
            _context.Counters.Add(item);
            await _context.SaveChangesAsync(cancellationToken);
            return await Result<int>.SuccessAsync(item.Id);
        }

    }
}

