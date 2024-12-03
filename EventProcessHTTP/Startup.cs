using EventProcessHTTP;

using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;

using System;
[assembly: FunctionsStartup(typeof(Startup))]
namespace EventProcessHTTP
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging();
            builder.Services.AddAzureClients(builder =>
            {
                builder.AddWebPubSubServiceClient(Environment.GetEnvironmentVariable("WebPubSubConnectionString"), Environment.GetEnvironmentVariable("WebPubSubHubName"));
            });
        }
    }
}
