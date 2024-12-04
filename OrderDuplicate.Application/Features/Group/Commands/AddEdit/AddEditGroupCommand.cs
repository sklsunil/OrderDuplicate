using AutoMapper;
using MediatR;
using OrderDuplicate.Application.Common;
using OrderDuplicate.Application.Common.Exceptions;
using OrderDuplicate.Application.Common.Interfaces;
using OrderDuplicate.Application.Common.Interfaces.Caching;
using OrderDuplicate.Application.Common.Mappings;
using OrderDuplicate.Application.Features.Counter.DTOs;
using OrderDuplicate.Application.Features.Group.Caching;
using OrderDuplicate.Application.Features.Group.Dto;
using OrderDuplicate.Application.Features.Order.Caching;
using OrderDuplicate.Domain.Entities;
using OrderDuplicate.Domain.Events;
using System.ComponentModel;

namespace OrderDuplicate.Application.Features.Group.Commands.AddEdit
{
    public class AddEditGroupCommand : IMapFrom<GroupDto>, ICacheInvalidatorRequest<Result<int>>
    {
        [Description("Id")]
        public int Id { get; set; }
        public int CounterId { get; set; }
        public string GroupName { get; set; }
        public CounterDto Counter { get; set; }
        public string CacheKey => GroupCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => OrderCacheKey.SharedExpiryTokenSource();
    }
    public class AddEditGroupCommandHandler(
    IApplicationDbContext context,
    IMapper mapper
        ) : IRequestHandler<AddEditGroupCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        public async Task<Result<int>> Handle(AddEditGroupCommand request, CancellationToken cancellationToken)
        {
            var dto = _mapper.Map<GroupDto>(request);
            if (request.Id > 0)
            {
                var item = await _context.Groups.FindAsync(new object[] { request.Id }, cancellationToken) ?? throw new NotFoundException($"The Group [{request.Id}] was not found.");
                item = _mapper.Map(dto, item);
                // raise a update domain event
                item.AddDomainEvent(new UpdatedEvent<GroupEntity>(item));
                await _context.SaveChangesAsync(cancellationToken);
                return await Result<int>.SuccessAsync(item.Id);
            }
            else
            {
                var item = _mapper.Map<GroupEntity>(dto);
                // raise a create domain event
                item.AddDomainEvent(new CreatedEvent<GroupEntity>(item));
                _context.Groups.Add(item);
                await _context.SaveChangesAsync(cancellationToken);
                return await Result<int>.SuccessAsync(item.Id);
            }

        }
    }
}
