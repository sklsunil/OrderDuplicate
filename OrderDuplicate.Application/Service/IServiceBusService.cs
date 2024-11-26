namespace OrderDuplicate.Application.Service
{
    public interface IServiceBusService
    {
        public Task SendAsync<T>(T message, string sessionId);
    }
}
