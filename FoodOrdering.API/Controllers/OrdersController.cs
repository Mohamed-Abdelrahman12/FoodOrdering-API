using FoodOrdering.Application.Features.Orders.Commands.CancelOrder;
using FoodOrdering.Application.Features.Orders.Commands.PlaceOrder;
using FoodOrdering.Application.Features.Orders.Commands.UpdateOrderStatus;
using FoodOrdering.Application.Features.Orders.Queries.GetOrderById;
using FoodOrdering.Application.Features.Orders.Queries.GetUserOrders;
using FoodOrdering.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrdering.API.Controllers
{
    [Authorize]
    public class OrdersController : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> GetMyOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await Mediator.Send(new GetUserOrdersQuery(CurrentUserId, page, pageSize));
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await Mediator.Send(new GetOrderByIdQuery(id, CurrentUserId, IsAdmin));
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderRequest request)
        {
            var result = await Mediator.Send(new PlaceOrderCommand(
                CurrentUserId,
                request.DeliveryAddress,
                request.PaymentMethod,
                request.Notes
            ));
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("{id:guid}/cancel")]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var result = await Mediator.Send(new CancelOrderCommand(id, CurrentUserId, IsAdmin));
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{id:guid}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusRequest request)
        {
            var result = await Mediator.Send(new UpdateOrderStatusCommand(id, request.Status, CurrentUserId));
            return StatusCode(result.StatusCode, result);
        }
    }

    public record PlaceOrderRequest(string DeliveryAddress, PaymentMethod PaymentMethod, string? Notes);
    public record UpdateStatusRequest(OrderStatus Status);
}
