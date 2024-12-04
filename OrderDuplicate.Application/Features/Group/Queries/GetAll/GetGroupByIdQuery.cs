﻿using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderDuplicate.Application.Common;
using OrderDuplicate.Application.Common.Interfaces;
using OrderDuplicate.Application.Common.Interfaces.Caching;
using OrderDuplicate.Application.Features.Group.Caching;
using OrderDuplicate.Application.Features.Group.Dto;

namespace OrderDuplicate.Application.Features.Group.Queries.GetById;

public class GetGroupByIdQuery : ICacheInvalidatorRequest<Result<GroupDto>>
{
    public int Id { get; set; }
    public string CacheKey => GroupCacheKey.GetAllCacheKey;
    public CancellationTokenSource? SharedExpiryTokenSource => GroupCacheKey.SharedExpiryTokenSource();

}
public class GetGroupByIdQueryHandler(IApplicationDbContext context,
IMapper mapper
  ) :
       IRequestHandler<GetGroupByIdQuery, Result<GroupDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    public async Task<Result<GroupDto>> Handle(GetGroupByIdQuery request, CancellationToken cancellationToken)
    {
        var groupItems = await _context.Groups
            .FirstOrDefaultAsync(oli => oli.Id == request.Id, cancellationToken);

        var groupDtos = _mapper.Map<GroupDto>(groupItems);
        return Result<GroupDto>.Success(groupDtos);
    }
}