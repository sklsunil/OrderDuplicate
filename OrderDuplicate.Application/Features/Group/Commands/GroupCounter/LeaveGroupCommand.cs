using MediatR;

using Microsoft.EntityFrameworkCore;

using OrderDuplicate.Application.Common;
using OrderDuplicate.Application.Common.Exceptions;
using OrderDuplicate.Application.Common.Interfaces;
using OrderDuplicate.Application.Common.Interfaces.Caching;
using OrderDuplicate.Application.Features.Group.Caching;

namespace OrderDuplicate.Application.Features.Group.Commands.GroupCounter
{
    public class LeaveGroupCommand : ICacheInvalidatorRequest<Result<string>>
    {
        public int CounterId { get; set; }
        public int GroupId { get; set; }
        public string CacheKey => GroupCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => GroupCacheKey.SharedExpiryTokenSource();

    }
    public class LeaveGroupHandler(IApplicationDbContext context) : IRequestHandler<LeaveGroupCommand, Result<string>>
    {
        private readonly IApplicationDbContext _context = context;

        public async Task<Result<string>> Handle(LeaveGroupCommand request, CancellationToken cancellationToken)
        {
            var groupCounter = await _context.GroupCounters
                .FirstOrDefaultAsync(gc => gc.CounterId == request.CounterId && gc.GroupId == request.GroupId, cancellationToken);

            if (groupCounter == null)
            {
                throw new NotFoundException($"The Counter [{request?.CounterId}] was not found.");
            }

            _context.GroupCounters.Remove(groupCounter);
            await _context.SaveChangesAsync(cancellationToken);

            return await Result<string>.SuccessAsync("Removed Counter From Groups.");
        }
    }
}
