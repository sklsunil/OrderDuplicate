using Azure.Messaging.ServiceBus;

using Microsoft.Extensions.Logging;



namespace OrderDuplicate.Application.Service
{
    public class CustomerService(string connectionString, string topicName, ILogger<CustomerService> logger) : ICustomerService
    {
        private readonly string _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        private readonly string _topicName = topicName ?? throw new ArgumentNullException(nameof(topicName));
        private readonly ILogger<CustomerService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task StartListening(Action<string> onMessageReceived, string counterSessionId)
        {
            await using var client = new ServiceBusClient(_connectionString);

            // Explicitly accept the session
            ServiceBusSessionReceiver sessionReceiver;
            try
            {
                sessionReceiver = await client.AcceptSessionAsync(_topicName, "OrderSubscription", counterSessionId);
                _logger.LogInformation($"Accepted session with ID: {counterSessionId}");
            }
            catch (ServiceBusException ex) when (ex.Reason == ServiceBusFailureReason.SessionCannotBeLocked)
            {
                _logger.LogError($"Failed to lock session {counterSessionId}: {ex.Message}");
                throw;
            }

            try
            {
                while (true)
                {
                    ServiceBusReceivedMessage message = await sessionReceiver.ReceiveMessageAsync(TimeSpan.FromSeconds(10));

                    if (message == null) break; // No more messages in the session

                    try
                    {
                        string messageBody = message.Body.ToString();
                        onMessageReceived(messageBody);
                        await sessionReceiver.CompleteMessageAsync(message);
                        _logger.LogInformation($"Processed message for session {counterSessionId}: {messageBody}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Failed to process message in session {counterSessionId}");
                        await sessionReceiver.AbandonMessageAsync(message);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while processing session {counterSessionId}");
            }
            finally
            {
                await sessionReceiver.CloseAsync();
                _logger.LogInformation($"Session {counterSessionId} processing complete.");
            }
        }
    }

}
