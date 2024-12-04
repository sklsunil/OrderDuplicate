using OrderDuplicate.Domain.Entities;

using System.Runtime.InteropServices;

namespace OrderDuplicate.Infrastructure.Persistence
{

    public class ApplicationDbContextInitialiser
    {
        private readonly ILogger<ApplicationDbContextInitialiser> _logger;
        private readonly ApplicationDbContext _context;

        public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public async Task InitialiseAsync()
        {
            try
            {
                if (_context.Database.IsSqlServer())
                {
                    await _context.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initialising the database.");
                throw;
            }
        }
        public async Task SeedAsync()
        {
            try
            {
                await TrySeedAsync();
                await CreateGroup();
                _context.ChangeTracker.Clear();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }


        public async Task TrySeedAsync()
        {

            List<CounterEntity> Counters = new()
            {
                            new CounterEntity() { CounterName = "Counter 1", PersonId = 1  },
                            new CounterEntity() { CounterName = "Counter 2", PersonId = 2 },
                            new CounterEntity() { CounterName = "Counter 3", PersonId = 3 },
                            new CounterEntity() { CounterName = "Counter 4", PersonId = 4 }

            };
            foreach (var Counter in Counters)
            {
                var dbValue = await _context.Counters.FirstOrDefaultAsync(x => x.CounterName == Counter.CounterName);
                if (dbValue == null)
                {
                    await _context.Counters.AddAsync(Counter);
                    await _context.SaveChangesAsync();
                }
            }
        }
        public async Task CreateGroup()
        {
            List<GroupEntity> groupEntities = new List<GroupEntity>
            {
                new GroupEntity { GroupName = "Group 1" },
                new GroupEntity { GroupName = "Group 2" },
                new GroupEntity { GroupName = "Group 3" },
                new GroupEntity { GroupName = "Group 4" },
            };

            foreach (var group in groupEntities)
            {
                var dbValue = await _context.Groups.FirstOrDefaultAsync(x => x.GroupName == group.GroupName);
                if (dbValue == null)
                {
                    await _context.Groups.AddAsync(group);
                    await _context.SaveChangesAsync();

                    List<int> counterIds = group.GroupName switch
                    {
                        "Group 1" => new List<int> { 1, 2 },
                        "Group 2" => new List<int> { 2, 3 },
                        "Group 3" => new List<int> { 1, 2, 3 },
                        "Group 4" => new List<int> { 4 },
                        _ => new List<int>()
                    };

                    foreach (var counterId in counterIds)
                    {
                        var groupCounter = new GroupCounterEntity
                        {
                            GroupId = group.Id,
                            CounterId = counterId
                        };
                        await _context.GroupCounters.AddAsync(groupCounter);
                    }
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
