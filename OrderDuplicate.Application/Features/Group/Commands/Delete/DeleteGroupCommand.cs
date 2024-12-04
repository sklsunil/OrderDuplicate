using AutoMapper;
using MediatR;
using OrderDuplicate.Application.Common.Interfaces.Caching;
using OrderDuplicate.Application.Common.Interfaces;
using OrderDuplicate.Domain.Entities;
using OrderDuplicate.Domain.Events;
using OrderDuplicate.Application.Common;
using OrderDuplicate.Application.Features.Group.Caching;
using Microsoft.EntityFrameworkCore;

namespace OrderDuplicate.Application.Features.Group.Commands.Delete;

public class DeleteGroupCommand(List<int> id) : ICacheInvalidatorRequest<Result>
{
    public List<int> Id { get; } = id;
    public string CacheKey => GroupCacheKey.GetAllCacheKey;
    public CancellationTokenSource? SharedExpiryTokenSource => GroupCacheKey.SharedExpiryTokenSource();
}

public class DeleteGroupCommandHandler(
    IApplicationDbContext context,
     IMapper mapper
        ) :
             IRequestHandler<DeleteGroupCommand, Result>

{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<Result> Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
    {
        var items = await _context.Groups.Where(x => request.Id.Contains(x.Id)).ToListAsync(cancellationToken);
        foreach (var item in items)
        {
            // raise a delete domain event
            item.AddDomainEvent(new DeletedEvent<GroupEntity>(item));
            _context.Groups.Remove(item);
        }
        await _context.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync();
    }

}

