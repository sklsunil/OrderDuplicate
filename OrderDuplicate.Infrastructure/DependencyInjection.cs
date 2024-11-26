using OrderDuplicate.Application.Common.Interfaces;
using OrderDuplicate.Infrastructure.Extensions;
using OrderDuplicate.Infrastructure.Persistence.Interceptors;

using Microsoft.Extensions.Configuration;

namespace OrderDuplicate.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });


            services.AddScoped<AuditableEntitySaveChangesInterceptor>();

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                var isInMemoryDatabase = Convert.ToBoolean(configuration["UseInMemoryDatabase"]);
                if (isInMemoryDatabase)
                {
                    options.UseInMemoryDatabase("Counter");
                }
                else
                {
                    options.UseSqlServer(
                          configuration.GetConnectionString("DefaultConnection"),
                          builder =>
                          {
                              builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                              builder.EnableRetryOnFailure(maxRetryCount: 5,
                                                           maxRetryDelay: TimeSpan.FromSeconds(10),
                                                           errorNumbersToAdd: null);
                              builder.CommandTimeout(15);
                          });
                }
                options.EnableDetailedErrors(detailedErrorsEnabled: true);
                options.EnableSensitiveDataLogging();
            });

            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddScoped<IDbContextFactory<ApplicationDbContext>, OrderDuplicateContextFactory<ApplicationDbContext>>();
            services.AddTransient<IApplicationDbContext>(provider => provider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext());
            services.AddScoped<ApplicationDbContextInitialiser>();

            services.AddServices();
            services.AddControllers();
            return services;
        }
    }
}
