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

public class AddEditOrderItemCommand : IMapFrom<OrderItemDto>, ICacheInvalidatorRequest<Result<int>>
{
    [Description("Id")]
    public int Id { get; set; }
    public string OrderNumber { get; set; }
    public string CounterPersonId { get; set; }
    public bool IsCheckOut { get; set; }
    public string CacheKey => OrderCacheKey.GetAllCacheKey;
    public CancellationTokenSource? SharedExpiryTokenSource => OrderCacheKey.SharedExpiryTokenSource();
}

public class AddEditOrderItemCommandHandler(
    IApplicationDbContext context,
    IMapper mapper
        ) : IRequestHandler<AddEditOrderItemCommand, Result<int>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<Result<int>> Handle(AddEditOrderItemCommand request, CancellationToken cancellationToken)
    {
        var dto = _mapper.Map<OrderItemDto>(request);
        if (request.Id > 0)
        {
            var item = await _context.OrderItems.FindAsync(new object[] { request.Id }, cancellationToken) ?? throw new NotFoundException($"The Order [{request.Id}] was not found.");
            item = _mapper.Map(dto, item);
            // raise a update domain event
            item.AddDomainEvent(new UpdatedEvent<OrderLineItemEntity>(item));
            await _context.SaveChangesAsync(cancellationToken);
            return await Result<int>.SuccessAsync(item.Id);
        }
        else
        {
            var item = _mapper.Map<OrderLineItemEntity>(dto);
            // raise a create domain event
            item.AddDomainEvent(new CreatedEvent<OrderLineItemEntity>(item));
            _context.OrderItems.Add(item);
            await _context.SaveChangesAsync(cancellationToken);
            return await Result<int>.SuccessAsync(item.Id);
        }

    }
}

