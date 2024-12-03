using MediatR;

using Microsoft.AspNetCore.Mvc;

using OrderDuplicate.Application.Common.Models;
using OrderDuplicate.Application.Features.Counter.Commands.Create;
using OrderDuplicate.Application.Features.Counter.Commands.Delete;
using OrderDuplicate.Application.Features.Counter.Commands.Update;
using OrderDuplicate.Application.Features.Counter.Queries.GetAll;
using OrderDuplicate.Application.Features.Counter.Queries.Pagination;
using OrderDuplicate.Application.Model.Counter;

using System.Threading.Tasks.Dataflow;

namespace OrderDuplicate.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CounterController(ILogger<CounterController> logger, IMediator mediator) : ControllerBase
    {

        private readonly ILogger<CounterController> _logger = logger;
        public readonly IMediator _mediator = mediator;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] PaginationFilter request)
        {
            var query = new CounterPaginationQuery
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

        [HttpGet("[action]")]
        public async Task<IActionResult> GetByName([FromQuery] string name)
        {
            var query = new CounterGetByNameQuery { Name = name };
            return Ok(await _mediator.Send(query).ConfigureAwait(false));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] CounterModel model)
        {
            var cmd = new CreateCounterCommand { CounterName = model.CounterName, PersonId = model.PersonId };
            return Ok(await _mediator.Send(cmd).ConfigureAwait(false));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync([FromRoute] int id, [FromBody] UpdateCounterModel model)
        {
            if (id != model.Id)
            {
                return BadRequest(await Result.FailureAsync(new List<string> { "Invalid Id" }));
            }
            var cmd = new UpdateCounterCommand { Id = model.Id, PersonId = model.PersonId, CounterName = model.CounterName };
            return Ok(await _mediator.Send(cmd).ConfigureAwait(false));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            var cmd = new DeleteCounterCommand(new List<int> { id });
            return Ok(await _mediator.Send(cmd).ConfigureAwait(false));
        }

    }
}