using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Azure.Messaging.WebPubSub;
using EventProcessHTTP.Model;

namespace EventProcessHTTP
{
    public class EventProcess(WebPubSubServiceClient client)
    {
        public readonly WebPubSubServiceClient _client = client;

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
            var eventData = context.GetInput<dynamic>();

            // Call the activity function
            await context.CallActivityAsync("SendEventFunction", eventData);
        }

        [FunctionName("SendEventFunction")]
        public async Task RunActivity([ActivityTrigger] IDurableActivityContext context, ILogger log)
        {
            log.LogInformation("Activity function triggered with data.");
           
            //var taskProgress = context.GetInput<TaskProgress>();

            //// Logic to process the event and send it to Pub/Sub
            //await _client.SendToUserAsync(counterId.ToString(), content, contentType: ContentType.ApplicationJson);

            log.LogInformation("Event sent to Pub/Sub.");
        }

    }
}
