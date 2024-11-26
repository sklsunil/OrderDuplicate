using AutoMapper;

using OrderDuplicate.Application.Common;
using OrderDuplicate.Application.Common.Exceptions;
using OrderDuplicate.Application.Common.Interfaces;
using OrderDuplicate.Application.Common.Interfaces.Caching;
using OrderDuplicate.Application.Common.Mappings;
using OrderDuplicate.Application.Features.Order.DTOs;
using OrderDuplicate.Domain.Entities;
using OrderDuplicate.Domain.Events;

using MediatR;

using System.ComponentModel;
using OrderDuplicate.Application.Features.Order.Caching;

namespace OrderDuplicate.Application.Features.Order.Commands.AddEdit;

public class AddEditOrderCommand : IMapFrom<OrderDto>, ICacheInvalidatorRequest<Result<int>>
{
    [Description("Id")]
    public int Id { get; set; }
    public string OrderNumber { get; set; }
    public string CounterPersonId { get; set; }
    public bool IsCheckOut { get; set; }
    public string CacheKey => OrderCacheKey.GetAllCacheKey;
    public CancellationTokenSource? SharedExpiryTokenSource => OrderCacheKey.SharedExpiryTokenSource();
}

public class AddEditOrderCommandHandler(
    IApplicationDbContext context,
    IMapper mapper
        ) : IRequestHandler<AddEditOrderCommand, Result<int>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<Result<int>> Handle(AddEditOrderCommand request, CancellationToken cancellationToken)
    {
        var dto = _mapper.Map<OrderDto>(request);
        if (request.Id > 0)
        {
            var item = await _context.Orders.FindAsync(new object[] { request.Id }, cancellationToken) ?? throw new NotFoundException($"The Order [{request.Id}] was not found.");
            item = _mapper.Map(dto, item);
            // raise a update domain event
            item.AddDomainEvent(new UpdatedEvent<OrderEntity>(item));
            await _context.SaveChangesAsync(cancellationToken);
            return await Result<int>.SuccessAsync(item.Id);
        }
        else
        {
            var item = _mapper.Map<OrderEntity>(dto);
            // raise a create domain event
            item.AddDomainEvent(new CreatedEvent<OrderEntity>(item));
            _context.Orders.Add(item);
            await _context.SaveChangesAsync(cancellationToken);
            return await Result<int>.SuccessAsync(item.Id);
        }

    }
}

