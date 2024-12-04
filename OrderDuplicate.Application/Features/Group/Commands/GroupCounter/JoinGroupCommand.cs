using MediatR;

using Microsoft.EntityFrameworkCore;

using OrderDuplicate.Application.Common;
using OrderDuplicate.Application.Common.Interfaces;
using OrderDuplicate.Application.Common.Interfaces.Caching;
using OrderDuplicate.Application.Features.Group.Caching;
using OrderDuplicate.Domain.Entities;

namespace OrderDuplicate.Application.Features.Group.Commands.GroupCounter
{
    public class JoinGroupCommand : ICacheInvalidatorRequest<Result<string>>
    {
        public int CounterId { get; set; }
        public int GroupId { get; set; }
        public string CacheKey => GroupCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => GroupCacheKey.SharedExpiryTokenSource();

    }
    public class JoinGroupHandler(IApplicationDbContext context) : IRequestHandler<JoinGroupCommand, Result<string>>
    {
        private readonly IApplicationDbContext _context = context;

        public async Task<Result<string>> Handle(JoinGroupCommand request, CancellationToken cancellationToken)
        {
            // Check if the GroupCounterEntity already exists
            var existingGroupCounter = await _context.GroupCounters
                .FirstOrDefaultAsync(gc => gc.CounterId == request.CounterId && gc.GroupId == request.GroupId, cancellationToken);

            if (existingGroupCounter != null)
            {
                return await Result<string>.FailureAsync(new[] { "GroupCounter already exists." });
            }

            var groupCounter = new GroupCounterEntity
            {
                CounterId = request.CounterId,
                GroupId = request.GroupId
            };

            _context.GroupCounters.Add(groupCounter);
            await _context.SaveChangesAsync(cancellationToken);

            return await Result<string>.SuccessAsync("Joined Group.");
        }
    }
}
