using FluentValidation;

using MediatR;

using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using OrderDuplicate.Application.Common.Behaviours;
using OrderDuplicate.Application.Service;

using System.Reflection;

namespace OrderDuplicate.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(MemoryCacheBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CacheInvalidationBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
            services.AddLazyCache();
            //services.AddScoped<_test_FluentValidator>(); 
            services.AddAzureClients(builder =>
            {
                builder.AddWebPubSubServiceClient(configuration["WebPubSub:ConnectionString"], configuration["WebPubSub:HubName"]);
            });          
            return services;
        }
    }
}
