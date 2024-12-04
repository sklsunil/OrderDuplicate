using AutoMapper;
using MediatR;
using OrderDuplicate.Application.Common.Interfaces.Caching;
using OrderDuplicate.Application.Common.Interfaces;
using OrderDuplicate.Domain.Entities;
using OrderDuplicate.Domain.Events;
using System.ComponentModel;
using OrderDuplicate.Application.Common;
using OrderDuplicate.Application.Features.Group.Caching;
using OrderDuplicate.Application.Features.Counter.DTOs;

namespace OrderDuplicate.Application.Features.Group.Commands.Update
{
    public class UpdateGroupCommand : ICacheInvalidatorRequest<Result>
    {
        [Description("Id")]
        public int Id { get; set; }
        [Description("GroupName")]
        public string GroupName { get; set; }
        public string CacheKey => GroupCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => GroupCacheKey.SharedExpiryTokenSource();
    }

    public class UpdateGroupCommandHandler(
        IApplicationDbContext context,
         IMapper mapper
            ) : IRequestHandler<UpdateGroupCommand, Result>
    {
        private readonly IApplicationDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        public async Task<Result> Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
        {
            var item = await _context.Groups.FindAsync(new object[] { request.Id }, cancellationToken);
            if (item != null)
            {
                item.GroupName = request.GroupName;
                // raise a update domain event
                item.AddDomainEvent(new UpdatedEvent<GroupEntity>(item));
                await _context.SaveChangesAsync(cancellationToken);
            }
            return await Result.SuccessAsync();
        }
    }
}
