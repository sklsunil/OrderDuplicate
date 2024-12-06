using Azure.Core;
using Azure.Messaging.WebPubSub;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using OrderDuplicate.Application.Features.Group.Queries.GetById;
using OrderDuplicate.Application.Features.Order.DTOs;
using OrderDuplicate.Domain.Models;

using System.Text.Json;

namespace OrderDuplicate.Server.Controllers
{
    /// <summary>
    /// Controller for handling WebPubSub related operations.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class WebPubSubController(ILogger<OrderController> logger, WebPubSubServiceClient client, IConfiguration configuration, IMediator mediator) : ControllerBase
    {
        private readonly ILogger<OrderController> _logger = logger;
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
            var serviceClient = await _client.GetClientAccessUriAsync(userId: counterId.ToString());
            return Ok(serviceClient.AbsoluteUri);
        }

        /// <summary>
        /// Handles the event options request.
        /// </summary>
        /// <returns>Allowed origin for the webhook.</returns>
        [HttpOptions("[action]")]
        public IActionResult EventHandler()
        {
            var allowedOrigin = "pubsub-test-poc.webpubsub.azure.com";

            Response.Headers["WebHook-Allowed-Origin"] = allowedOrigin;
            return Ok();
        }

        /// <summary>
        /// Handles the event post request for a specific counter.
        /// </summary>
        /// <param name="counterId">The ID of the counter.</param>
        /// <param name="order">The order details.</param>
        /// <returns>Result of the operation.</returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> EventHandler([FromQuery] int counterId, [FromBody] OrderDto order)
        {
            var content = JsonSerializer.Serialize(order);
            await _client.SendToUserAsync(counterId.ToString(), content, contentType: ContentType.ApplicationJson);

            return Ok();
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

        /// <summary>
        /// Notifies all clients that the counter is closed.
        /// </summary>
        /// <returns>Result of the operation.</returns>
        [HttpGet("[action]")]
        public async Task<IActionResult> NotifyAll()
        {
            await _client.SendToAllAsync("Counter Closed!", contentType: ContentType.TextPlain);
            return Ok();
        }

        /// <summary>
        /// Removes a counter from all groups.
        /// </summary>
        /// <param name="counterId">The ID of the counter.</param>
        /// <returns>Result of the operation.</returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> RemoveCounterFromAllGroupsAsync([FromQuery] int counterId)
        {
            await _client.RemoveUserFromAllGroupsAsync($"{counterId}");
            return Ok();
        }

        /// <summary>
        /// Adds a counter to a specific group.
        /// </summary>
        /// <param name="counterId">The ID of the counter.</param>
        /// <param name="groupId">The ID of the group.</param>
        /// <returns>Result of the operation.</returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> JoinGroupAsync([FromQuery] int counterId, [FromQuery] int groupId)
        {
            await _client.AddUserToGroupAsync(group: "group " + groupId, userId: $"{counterId}");
            return Ok();
        }

        /// <summary>
        /// Removes a counter from a specific group.
        /// </summary>
        /// <param name="counterId">The ID of the counter.</param>
        /// <param name="groupId">The ID of the group.</param>
        /// <returns>Result of the operation.</returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> LeaveGroupAsync([FromQuery] int counterId, [FromQuery] int groupId)
        {
            await _client.RemoveUserFromGroupAsync(group: "group " + groupId, userId: $"{counterId}");
            return Ok();
        }

        /// <summary>
        /// Sends an event to a specific group.
        /// </summary>
        /// <param name="groupId">The ID of the group.</param>
        /// <param name="event">The event details.</param>
        /// <returns>Result of the operation.</returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> SendToGroupAsync([FromQuery] int groupId, [FromBody] PubSubEvent @event)
        {
            var query = new GetGroupByIdQuery { Id = groupId };
            var group = await _mediator.Send(query).ConfigureAwait(false);
            if (group?.Data == null)
            {
                return NotFound();
            }
            List<string> counters = group.Data.Counters.Select(x => x.Id.ToString()).ToList();
            foreach (var user in counters)
            {
                if (await _client.UserExistsAsync(user))
                {
                    if (!await _client.GroupExistsAsync(user))
                    {
                        await _client.AddUserToGroupAsync(group: "group " + groupId, userId: $"{user}");
                    }
                    await _client.SendToGroupAsync(group: "group " + groupId, content: JsonSerializer.Serialize(@event));
                }
            }
            return Ok();
        }
    }
}
