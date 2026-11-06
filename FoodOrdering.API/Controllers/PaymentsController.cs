using FoodOrdering.Application.Features.Payments.Commands.ProcessOnlinePayment;
using FoodOrdering.Application.Features.Payments.Queries.GetPaymentByOrder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrdering.API.Controllers
{
    [Authorize]
    public class PaymentsController : BaseController
    {
        [HttpGet("order/{orderId:guid}")]
        public async Task<IActionResult> GetByOrder(Guid orderId)
        {
            var result = await Mediator.Send(new GetPaymentByOrderQuery(orderId, CurrentUserId, IsAdmin));
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("order/{orderId:guid}/pay-online")]
        public async Task<IActionResult> PayOnline(Guid orderId, [FromBody] OnlinePaymentRequest request)
        {
            var result = await Mediator.Send(new ProcessOnlinePaymentCommand(
                orderId,
                CurrentUserId,
                request.CardNumber,
                request.CardHolder,
                request.ExpiryMonth,
                request.ExpiryYear,
                request.Cvv
            ));
            return StatusCode(result.StatusCode, result);
        }
    }

    public record OnlinePaymentRequest(
        string CardNumber,
        string CardHolder,
        string ExpiryMonth,
        string ExpiryYear,
        string Cvv
    );

}
