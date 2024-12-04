using MediatR;

using Microsoft.EntityFrameworkCore;

using OrderDuplicate.Application.Common;
using OrderDuplicate.Application.Common.Exceptions;
using OrderDuplicate.Application.Common.Interfaces;
using OrderDuplicate.Application.Common.Interfaces.Caching;
using OrderDuplicate.Application.Features.Group.Caching;

namespace OrderDuplicate.Application.Features.Group.Commands.GroupCounter
{
    public class RemoveCounterFromAllGroupsCommand : ICacheInvalidatorRequest<Result<string>>
    {
        public int CounterId { get; set; }
        public string CacheKey => GroupCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => GroupCacheKey.SharedExpiryTokenSource();

    }
    public class RemoveCounterFromAllGroupsHandler(IApplicationDbContext context) : IRequestHandler<RemoveCounterFromAllGroupsCommand, Result<string>>
    {
        private readonly IApplicationDbContext _context = context;

        public async Task<Result<string>> Handle(RemoveCounterFromAllGroupsCommand request, CancellationToken cancellationToken)
        {
            var counter = await _context.Counters
                .Include(c => c.GroupCounters)
                .FirstOrDefaultAsync(c => c.Id == request.CounterId, cancellationToken);

            if (counter == null)
            {
                throw new NotFoundException($"The Counter [{request?.CounterId}] was not found.");
            }

            _context.GroupCounters.RemoveRange(counter.GroupCounters);
            await _context.SaveChangesAsync(cancellationToken);

            return await Result<string>.SuccessAsync("Removed Counter From All Groups.");
        }
    }
}
