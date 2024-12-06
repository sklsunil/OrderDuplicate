using Azure.Messaging.WebPubSub;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using OrderDuplicate.Application.Common.Models;
using OrderDuplicate.Application.Features.Group.Commands.Create;
using OrderDuplicate.Application.Features.Group.Commands.Delete;
using OrderDuplicate.Application.Features.Group.Commands.GroupCounter;
using OrderDuplicate.Application.Features.Group.Commands.Update;
using OrderDuplicate.Application.Features.Group.Queries.GetAll;
using OrderDuplicate.Application.Features.Group.Queries.GetById;
using OrderDuplicate.Application.Features.Group.Queries.Pagination;
using OrderDuplicate.Application.Model.Group;

namespace OrderDuplicate.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GroupController(ILogger<GroupController> logger, WebPubSubServiceClient client, IMediator mediator) : ControllerBase
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
            var cmd = new CreateGroupCommand { GroupName = model.GroupName };
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
            var cmd = new UpdateGroupCommand { Id = model.Id, GroupName = model.GroupName };
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


        [HttpGet("[action]")]
        public async Task<IActionResult> RemoveCounterFromAllGroupsAsync([FromQuery] int counterId)
        {
            var command = new RemoveCounterFromAllGroupsCommand { CounterId = counterId };
            var result = await _mediator.Send(command).ConfigureAwait(false);
            await _client.RemoveUserFromAllGroupsAsync($"{counterId}");
            return Ok(result);

        }



        [HttpGet("[action]")]
        public async Task<IActionResult> JoinGroupAsync([FromQuery] int counterId, [FromQuery] int groupId)
        {
            var command = new JoinGroupCommand { CounterId = counterId, GroupId = groupId };
            var result = await _mediator.Send(command).ConfigureAwait(false);           
            return Ok(result);
        }


        [HttpGet("[action]")]
        public async Task<IActionResult> LeaveGroupAsync([FromQuery] int counterId, [FromQuery] int groupId)
        {
            var command = new LeaveGroupCommand { CounterId = counterId, GroupId = groupId };
            var result = await _mediator.Send(command).ConfigureAwait(false);
            return Ok(result);
        }
    }
}