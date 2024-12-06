using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using EventProcessHTTP.Model;
using System.Collections.Generic;
using Azure.Messaging.WebPubSub;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.IO;
using System.Threading;
using Newtonsoft.Json;

namespace EventProcessHTTP
{
    public class EventProcess
    {
        private readonly WebPubSubServiceClient _client;
        private static readonly List<string> AdUrls = new List<string>
        {
            "https://i.pinimg.com/originals/8d/37/d1/8d37d10f8fa4c608e5ecbcbdef2ad8a8.gif",
            "https://cdn.pixabay.com/animation/2023/02/13/14/04/14-04-44-438_512.gif",
            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcT1pEuNVWTzuoCUlzDVJAqu8KJtLjmedA_kpw&s",
            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcS-dLOeD1o4LbycEVWi06KS-CFGWclKMyGMXA&s",
            "https://www.shutterstock.com/shutterstock/videos/1107298053/thumb/7.jpg?ip=x480",
            "https://www.shutterstock.com/shutterstock/videos/1106439241/thumb/3.jpg?ip=x480",
            "https://img.freepik.com/free-vector/super-sale-background_23-2147825683.jpg",
            "https://mir-s3-cdn-cf.behance.net/project_modules/hd/59c4c059594379.5a2805b23d18b.gif",
            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRdrIcuDoY9XLbhfWHsADAiE15Y5vmpZuZI0A&s",
            "https://cdn.dribbble.com/users/206755/screenshots/14329028/media/4b01e380eb63e352132b18cb379a437d.gif"
        };

        public EventProcess(WebPubSubServiceClient client)
        {
            _client = client;
        }

        [FunctionName("HttpTriggerFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            log.LogInformation("HTTP trigger received a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            FunctionPubSubEvent data = JsonConvert.DeserializeObject<FunctionPubSubEvent>(requestBody);

            string instanceId = await starter.StartNewAsync("OrchestratorFunction", data);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }

        [FunctionName("OrchestratorFunction")]
        public async Task RunOrchestrator([OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var eventData = context.GetInput<FunctionPubSubEvent>();

            // Start a loop to send ads every 10 seconds
            while (true)
            {
                // Call the activity function to send ads
                var result = await context.CallActivityAsync<bool>("SendAdsFunction", eventData);
                if (!result)
                    break;
                // Wait for 10 seconds
                await context.CreateTimer(context.CurrentUtcDateTime.AddSeconds(10), CancellationToken.None);
            }
        }

        [FunctionName("SendAdsFunction")]
        public async Task<bool> SendAdsActivity([ActivityTrigger] FunctionPubSubEvent eventData, ILogger log)
        {
            log.LogInformation("SendAdsActivity triggered.");

            var counterId = eventData.SesstionId.ToString();

            if (!await _client.UserExistsAsync(counterId))
            {
                log.LogInformation($"User with CounterId {counterId} does not exist.");
                return false;
            }

            // Randomly select two different ads
            var rand = new Random();
            int adIndex1 = rand.Next(AdUrls.Count);
            int adIndex2;
            do
            {
                adIndex2 = rand.Next(AdUrls.Count);
            } while (adIndex2 == adIndex1); // Ensure two different ads

            // Create PubSubEvent for Ad 1
            var adEvent1 = new PubSubEvent
            {
                EventType = EventType.Custom,
                Content = JsonConvert.SerializeObject(new
                {
                    action = "updateAd",
                    adId = 1,
                    imageUrl = AdUrls[adIndex1]
                }),
                Identifier = "AdUpdate1",
                ControlType = ControlType.Default
            };

            // Create PubSubEvent for Ad 2
            var adEvent2 = new PubSubEvent
            {
                EventType = EventType.Custom,
                Content = JsonConvert.SerializeObject(new
                {
                    action = "updateAd",
                    adId = 2,
                    imageUrl = AdUrls[adIndex2]
                }),
                Identifier = "AdUpdate2",
                ControlType = ControlType.Default
            };

            var random = new Random();
            int ranVal = random.Next(2);
            if (ranVal == 1)
            {
                // Send Ad 1
                await _client.SendToUserAsync(counterId, JsonConvert.SerializeObject(adEvent1));
            }
            else
            {
                // Send Ad 2
                await _client.SendToUserAsync(counterId, JsonConvert.SerializeObject(adEvent2));
            }

            log.LogInformation($"Sent ads {adIndex1 + 1} and {adIndex2 + 1} to CounterId {counterId}.");
            return true;
        }
    }
}
