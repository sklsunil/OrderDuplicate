using Azure.Core;
using Azure.Messaging.WebPubSub;

using Microsoft.AspNetCore.Mvc;

using OrderDuplicate.Application.Features.Order.DTOs;

using System.Text.Json;

namespace OrderDuplicate.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WebPubSubController(ILogger<OrderController> logger, WebPubSubServiceClient client) : ControllerBase
    {
        public readonly ILogger<OrderController> _logger = logger;
        public readonly WebPubSubServiceClient _client = client;

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

        [HttpGet("[action]")]
        public async Task<IActionResult> NotifyAll()
        {
            await _client.SendToAllAsync("Counter Closed!", contentType: ContentType.TextPlain);
            return Ok();
        }
    }
}
