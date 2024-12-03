using Azure.Core;
using Azure.Messaging.WebPubSub;

using Microsoft.AspNetCore.Mvc;

using OrderDuplicate.Application.Features.Order.DTOs;
using OrderDuplicate.Client.Pages;
using OrderDuplicate.Domain.Models;

using System.Net.Http;
using System.Text.Json;

namespace OrderDuplicate.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WebPubSubController(ILogger<OrderController> logger, WebPubSubServiceClient client, IConfiguration configuration) : ControllerBase
    {
        public readonly ILogger<OrderController> _logger = logger;
        public readonly WebPubSubServiceClient _client = client;
        private readonly IConfiguration _configuration = configuration;

        [HttpGet("[action]")]
        public async Task<IActionResult> Negotiate([FromQuery] int counterId)
        {
            var serviceClient = await _client.GetClientAccessUriAsync(userId: counterId.ToString());
            return Ok(serviceClient.AbsoluteUri);
        }

        [HttpOptions("[action]")]
        public IActionResult EventHandler()
        {
            var allowedOrigin = "pubsub-test-poc.webpubsub.azure.com";

            Response.Headers["WebHook-Allowed-Origin"] = allowedOrigin;
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> EventHandler([FromQuery] int counterId, [FromBody] OrderDto order)
        {
            var content = JsonSerializer.Serialize(order);
            await _client.SendToUserAsync(counterId.ToString(), content, contentType: ContentType.ApplicationJson);

            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SendEventHanlder([FromQuery] int counterId, [FromBody] PubSubEvent @event)
        {
            var content = JsonSerializer.Serialize(@event);
            await _client.SendToUserAsync(counterId.ToString(), content, contentType: ContentType.ApplicationJson);
            
            var functionUrl = _configuration["AzureFunctionUrl"];
            if (!string.IsNullOrEmpty(functionUrl))
            {
                using var _httpClient = new HttpClient() { BaseAddress = new Uri(functionUrl) };
                // Trigger the Azure Function via HTTP
                var functionContent = new FunctionPubSubEvent 
                {
                    Content = @event.Content,
                    EventType = @event.EventType,
                    Identifier = @event.Identifier,
                    SesstionId = counterId
                };
                var response = await _httpClient.PostAsync("HttpTriggerFunction", new StringContent(JsonSerializer.Serialize(functionContent), System.Text.Encoding.UTF8, "application/json"));
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Failed to trigger Azure Function. Status Code: {response.StatusCode}");
                    return StatusCode((int)response.StatusCode, "Failed to trigger Azure Function");
                }
            }
            else
            {
                _logger.LogError("Azure Function URL is not configured.");
                return BadRequest("Azure Function URL is not configured.");
            }
           
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> NotifyAll()
        {
            await _client.SendToAllAsync("Counter Closed!", contentType: ContentType.TextPlain);
            return Ok();
        }


    }
}
