using AutoMapper;
using MediatR;
using OrderDuplicate.Application.Common.Interfaces.Caching;
using OrderDuplicate.Application.Common.Interfaces;
using OrderDuplicate.Application.Common.Mappings;
using OrderDuplicate.Domain.Entities;
using OrderDuplicate.Domain.Events;
using OrderDuplicate.Application.Features.Group.Dto;
using OrderDuplicate.Application.Features.Counter.DTOs;
using OrderDuplicate.Application.Features.Group.Caching;
using OrderDuplicate.Application.Common;

namespace OrderDuplicate.Application.Features.Group.Commands.Create;

public class CreateGroupCommand : IMapFrom<GroupDto>, ICacheInvalidatorRequest<Result<int>>
{
    public int CounterId { get; set; }
    public string GroupName { get; set; }
    public CounterDto Counter { get; set; }
    public string CacheKey => GroupCacheKey.GetAllCacheKey;
    public CancellationTokenSource? SharedExpiryTokenSource => GroupCacheKey.SharedExpiryTokenSource();
}

public class CreateGroupCommandHandler(
    IApplicationDbContext context,
    IMapper mapper
        ) : IRequestHandler<CreateGroupCommand, Result<int>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<Result<int>> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        var dto = _mapper.Map<GroupDto>(request);
        var item = _mapper.Map<GroupEntity>(dto);
        // raise a create domain event
        item.AddDomainEvent(new CreatedEvent<GroupEntity>(item));
        _context.Groups.Add(item);
        await _context.SaveChangesAsync(cancellationToken);
        return await Result<int>.SuccessAsync(item.Id);
    }
}


