using Azure.Core;
using Azure.Messaging.WebPubSub;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using OrderDuplicate.Domain.Models;

using System.Text.Json;

namespace OrderDuplicate.Server.Controllers
{
    /// <summary>
    /// Controller for handling WebPubSub related operations.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class WebPubSubController(ILogger<WebPubSubController> logger, WebPubSubServiceClient client, IConfiguration configuration, IMediator mediator) : ControllerBase
    {
        private readonly ILogger<WebPubSubController> _logger = logger;
        private readonly WebPubSubServiceClient _client = client;
        private readonly IConfiguration _configuration = configuration;
        public readonly IMediator _mediator = mediator;

        /// <summary>
        /// Negotiates the connection for a specific counter.
        /// </summary>
        /// <param name="counterId">The ID of the counter.</param>
        /// <returns>The access URI for the client.</returns>
        [HttpGet("[action]")]
        public async Task<IActionResult> Negotiate([FromQuery] int counterId)
        {
            TimeSpan expiry = TimeSpan.FromHours(1);
            if (!string.IsNullOrWhiteSpace(_configuration["SocketExpiry"]))
            {
                expiry = TimeSpan.FromHours(int.Parse(_configuration["SocketExpiry"]));
            }

            var serviceClient = await _client.GetClientAccessUriAsync(expiresAfter: expiry, userId: counterId.ToString());
            _logger.LogInformation("Connection created for session:" + counterId);
            return Ok(serviceClient.AbsoluteUri);
        }

        /// <summary>
        /// Sends an event to a specific counter and triggers an Azure Function.
        /// </summary>
        /// <param name="counterId">The ID of the counter.</param>
        /// <param name="event">The event details.</param>
        /// <returns>Result of the operation.</returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> SendEventHanlder([FromQuery] int counterId, [FromBody] PubSubEvent @event)
        {
            var content = JsonSerializer.Serialize(@event);
            _logger.LogInformation("event is:'" + content + "'\n Session is:" + counterId);
            await _client.SendToUserAsync(counterId.ToString(), content, contentType: ContentType.ApplicationJson);

            var functionUrl = _configuration["SendEventProcessURL"];
            if (!string.IsNullOrEmpty(functionUrl))
            {
                using var _httpClient = new HttpClient();
                // Trigger the Azure Function via HTTP
                var functionContent = new FunctionPubSubEvent
                {
                    Content = @event.Content,
                    EventType = @event.EventType,
                    Identifier = @event.Identifier,
                    SesstionId = counterId
                };
                var response = await _httpClient.PostAsync(functionUrl, new StringContent(JsonSerializer.Serialize(functionContent), System.Text.Encoding.UTF8, "application/json"));
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
    }
}
