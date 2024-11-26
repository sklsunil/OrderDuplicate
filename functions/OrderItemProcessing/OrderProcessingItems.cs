using Azure.Messaging.ServiceBus;

using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

using System.Threading.Tasks;

namespace OrderItemProcessing
{
    public class OrderProcessingItems
    {
        [FunctionName("OrderProcessing")]
        public async Task Run([ServiceBusTrigger("OrderUpdates", "OrderSubscription", Connection = "")] ServiceBusReceivedMessage message,
            ServiceBusClient client,
            ILogger log)
        {
            // Perform any processing or validation here.
            log.LogInformation($"Message received: {message.Body.ToString()}");

            var sessionId = message.SessionId;
            ServiceBusSender sender = client.CreateSender("OrderUpdates");
            var modifiedMessage = new ServiceBusMessage(message.Body)
            {
                SessionId = sessionId,
                // add any property
            };

            await sender.SendMessageAsync(modifiedMessage);
        }
    }
}
