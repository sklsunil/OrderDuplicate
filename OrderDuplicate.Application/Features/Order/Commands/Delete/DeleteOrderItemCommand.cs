using AutoMapper;

using OrderDuplicate.Application.Common;
using OrderDuplicate.Application.Common.Interfaces;
using OrderDuplicate.Application.Common.Interfaces.Caching;
using OrderDuplicate.Domain.Entities;
using OrderDuplicate.Domain.Events;

using MediatR;

using Microsoft.EntityFrameworkCore;
using OrderDuplicate.Application.Features.Order.Caching;

namespace OrderDuplicate.Application.Features.Order.Commands.Delete;

public class DeleteOrderItemCommand(List<int> id) : ICacheInvalidatorRequest<Result>
{
    public List<int> Id { get; } = id;
    public string CacheKey => OrderCacheKey.GetAllCacheKey;
    public CancellationTokenSource? SharedExpiryTokenSource => OrderCacheKey.SharedExpiryTokenSource();
}

public class DeleteOrderItemCommandHandler(
    IApplicationDbContext context,
     IMapper mapper
        ) :
             IRequestHandler<DeleteOrderItemCommand, Result>

{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<Result> Handle(DeleteOrderItemCommand request, CancellationToken cancellationToken)
    {
        var items = await _context.OrderItems.Where(x => request.Id.Contains(x.Id)).ToListAsync(cancellationToken);
        foreach (var item in items)
        {
            // raise a delete domain event
            item.AddDomainEvent(new DeletedEvent<OrderLineItemEntity>(item));
            _context.OrderItems.Remove(item);
        }
        await _context.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync();
    }

}

