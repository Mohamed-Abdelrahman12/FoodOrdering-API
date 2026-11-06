
using FoodOrdering.Application.Features.Cart.Queries.GetCart;
using FoodOrdering.Application.Features.Carts.Commands.AddToCart;
using FoodOrdering.Application.Features.Carts.Commands.ClearCart;
using FoodOrdering.Application.Features.Carts.Commands.RemoveCartItem;
using FoodOrdering.Application.Features.Carts.Commands.UpdateCartItem;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrdering.API.Controllers
{

    [Authorize]
    public class CartController : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var result = await Mediator.Send(new GetCartQuery(CurrentUserId));
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddItem([FromBody] AddToCartRequest request)
        {
            var result = await Mediator.Send(new AddToCartCommand(CurrentUserId, request.ProductId, request.Quantity));
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("items/{productId:guid}")]
        public async Task<IActionResult> UpdateItem(Guid productId, [FromBody] UpdateCartItemRequest request)
        {
            var result = await Mediator.Send(new UpdateCartItemCommand(CurrentUserId, productId, request.Quantity));
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("items/{productId:guid}")]
        public async Task<IActionResult> RemoveItem(Guid productId)
        {
            var result = await Mediator.Send(new RemoveCartItemCommand(CurrentUserId, productId));
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            var result = await Mediator.Send(new ClearCartCommand(CurrentUserId));
            return StatusCode(result.StatusCode, result);
        }
    }

    public record AddToCartRequest(Guid ProductId, int Quantity);
    public record UpdateCartItemRequest(int Quantity);
}
