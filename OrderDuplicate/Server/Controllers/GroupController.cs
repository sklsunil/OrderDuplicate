using Azure.Messaging.WebPubSub;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderDuplicate.Application.Common.Models;
using OrderDuplicate.Application.Features.Group.Commands.Create;
using OrderDuplicate.Application.Features.Group.Commands.Delete;
using OrderDuplicate.Application.Features.Group.Commands.Update;
using OrderDuplicate.Application.Features.Group.Queries.GetAll;
using OrderDuplicate.Application.Features.Group.Queries.GetById;
using OrderDuplicate.Application.Features.Group.Queries.Pagination;
using OrderDuplicate.Application.Model.Group;

namespace OrderDuplicate.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GroupController(ILogger<GroupController> logger, WebPubSubServiceClient client,  IMediator mediator) : ControllerBase
    {
        private readonly ILogger<GroupController> _logger = logger;
        public readonly IMediator _mediator = mediator;
        public readonly WebPubSubServiceClient _client = client;

        // Get All groups
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] PaginationFilter request)
        {
            var query = new GroupWithPaginationQuery
            {
                PageSize = request.PageSize,
                AdvancedSearch = request.AdvancedSearch,
                Keyword = request.Keyword,
                OrderBy = request.OrderBy,
                PageNumber = request.PageNumber,
                SortDirection = request.SortDirection,
            };
            return Ok(await _mediator.Send(query).ConfigureAwait(false));
        }
        // GetBy CounterId
        [HttpGet("[action]")]
        public async Task<IActionResult> GetByCounterId([FromQuery] int id)
        {
            var query = new GetGroupByCounterQuery { Id = id };
            return Ok(await _mediator.Send(query).ConfigureAwait(false));
        }

        // GetBy GroupId
        [HttpGet("[action]")]
        public async Task<IActionResult> GetByGroupId([FromQuery] int id)
        {
            var query = new GetGroupByIdQuery { Id = id };
            return Ok(await _mediator.Send(query).ConfigureAwait(false));
        }
        // Create group
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] GroupModel model)
        {
            var cmd = new CreateGroupCommand { GroupName = model.GroupName, CounterId = model.CounterId };
            return Ok(await _mediator.Send(cmd).ConfigureAwait(false));
        }

        // Update group
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync([FromRoute] int id, [FromBody] UpdateGroupModel model)
        {
            if (id != model.Id)
            {
                return BadRequest(await Result.FailureAsync(new List<string> { "Invalid Id" }));
            }
            var cmd = new UpdateGroupCommand { Id = model.Id, CounterId = model.CounterId, GroupName = model.GroupName };
            return Ok(await _mediator.Send(cmd).ConfigureAwait(false));
        }
        // Remove group
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            var cmd = new DeleteGroupCommand(new List<int> { id });
            return Ok(await _mediator.Send(cmd).ConfigureAwait(false));
        }

        // Remove user from all groups
        [HttpPost("[action]")]
        public async Task<IActionResult> RemoveUserFromAllGroups([FromQuery] string userId)
        {
            // Remove user from the table 
          //  await userService.RemoveUserAsync(userId);
            await _client.RemoveUserFromAllGroupsAsync(userId);
            return Ok("User removed from all groups.");
        }


        // join groups
        // save counter id in respective group
        // create client of web pubsub
        //  _client.AddUserToGroupAsync("", "");       
        [HttpPost("[action]")]
        public async Task<IActionResult> JoinGroup([FromQuery] string userId, [FromQuery] string groupId)
        {
            await _client.AddUserToGroupAsync(userId, groupId);
            return Ok("User added to group.");
        }

        // leave group
        // remove counter id from respective group
        // _client.RemoveUserFromGroupAsync("", "");
        [HttpPost("[action]")]
        public async Task<IActionResult> LeaveGroup([FromQuery] string userId, [FromQuery] string groupId)
        {
            await _client.RemoveUserFromGroupAsync(userId, groupId);
            return Ok("User removed from group.");
        }
    }
}
