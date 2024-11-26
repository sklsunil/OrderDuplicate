namespace OrderDuplicate.Application.Service
{
    public interface ICustomerService
    {
        Task StartListening(Action<string> onMessageReceived, string counterSessionId);
    }
}
