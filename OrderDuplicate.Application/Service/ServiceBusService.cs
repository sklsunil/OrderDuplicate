using Azure.Messaging.ServiceBus;

using Microsoft.Extensions.Logging;

using System.Text.Json;

namespace OrderDuplicate.Application.Service
{
    public class ServiceBusService : IServiceBusService
    {
        private readonly string _connectionString;
        private readonly string _topicName;
        private readonly ILogger<ServiceBusService> _logger;

        public ServiceBusService(string connectionString, string topicName, ILogger<ServiceBusService> logger)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _topicName = topicName ?? throw new ArgumentNullException(nameof(topicName));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SendAsync<T>(T message, string sessionId = null)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            // Create a Service Bus client
            await using var client = new ServiceBusClient(_connectionString);

            // Create a sender for the topic
            ServiceBusSender sender = client.CreateSender(_topicName);

            try
            {
                // Serialize the message to JSON
                string jsonMessage = JsonSerializer.Serialize(message);

                // Create a Service Bus message
                var serviceBusMessage = new ServiceBusMessage(jsonMessage);

                // Set the session ID if provided
                if (!string.IsNullOrEmpty(sessionId))
                {
                    serviceBusMessage.SessionId = sessionId;
                }

                // Send the message
                await sender.SendMessageAsync(serviceBusMessage);
                _logger.LogInformation("Message sent successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending the message.");
                throw;
            }
        }
    }
}
