using MediatR;

using Microsoft.AspNetCore.Mvc;

using OrderDuplicate.Application.Common.Models;
using OrderDuplicate.Application.Features.Order.Commands.Create;
using OrderDuplicate.Application.Features.Order.Commands.Delete;
using OrderDuplicate.Application.Features.Order.Queries.GetAll;
using OrderDuplicate.Application.Features.Order.Queries.Pagination;
using OrderDuplicate.Application.Model.Order;

namespace OrderDuplicate.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController(ILogger<OrderController> logger, IMediator mediator) : ControllerBase
    {
        private readonly ILogger<OrderController> _logger = logger;
        public readonly IMediator _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] PaginationFilter request)
        {
            var query = new OrderWithPaginationQuery
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

        [HttpGet("{counterId:int}")]
        public async Task<IActionResult> GetOrderByCounter([FromRoute] int counterId)
        {
            var query = new GetOrderByCounterQuery(counterId);
            return Ok(await _mediator.Send(query).ConfigureAwait(false));
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] OrderModel model)
        {
            var cmd = new CreateOrderCommand
            {
                OrderNumber = model.OrderNumber,
                CounterPersonId = model.CounterPersonId,
                IsCheckOut = true
            };

            var order = await _mediator.Send(cmd).ConfigureAwait(false);

            var itemModel = model.OrderItems.Select(x =>
                                    new CreateOrderItemCommand
                                    {
                                        OrderId = order.Data,
                                        Quantity = x.Quantity,
                                        Price = x.Price
                                    });

            await _mediator.Send(new CreateOrderItemsCommand { Items = itemModel.ToList() }).ConfigureAwait(false);

            return Ok(order);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            var cmd = new DeleteOrderCommand([id]);
            return Ok(await _mediator.Send(cmd).ConfigureAwait(false));
        }

        [HttpPost("{orderId}/items")]
        public async Task<IActionResult> CreateOrderItemAsync([FromRoute] int orderId, [FromBody] OrderItemModel model)
        {
            var cmd = new CreateOrderItemCommand
            {
                OrderId = orderId,
                Quantity = model.Quantity,
                Price = model.Price
            };

            return Ok(await _mediator.Send(cmd).ConfigureAwait(false));
        }

        [HttpGet("{orderId}/items")]
        public async Task<IActionResult> GetAllOrderItemsByOrderIdAsync([FromRoute] int orderId)
        {
            var query = new GetAllOrderItemsByOrderIdQuery(orderId);
            return Ok(await _mediator.Send(query).ConfigureAwait(false));
        }

        [HttpDelete("{orderId}/items/{itemId}")]
        public async Task<IActionResult> DeleteOrderItemAsync([FromRoute] int orderId, [FromRoute] int itemId)
        {
            var cmd = new DeleteOrderItemCommand([itemId]);
            return Ok(await _mediator.Send(cmd).ConfigureAwait(false));
        }
    }
}