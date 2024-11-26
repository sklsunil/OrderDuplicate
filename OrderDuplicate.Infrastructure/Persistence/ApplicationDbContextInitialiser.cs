using OrderDuplicate.Domain.Entities;

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
    }
}
