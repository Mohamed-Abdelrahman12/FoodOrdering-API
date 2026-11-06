using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Payments.DTOs;
using MediatR;

namespace FoodOrdering.Application.Features.Payments.Commands.ProcessOnlinePayment
{
    public record ProcessOnlinePaymentCommand(Guid OrderId, string UserId, string CardNumber, string CardHolder, string ExpiryMonth, string ExpiryYear, string Cvv)
      : IRequest<ApiResponse<PaymentDto>>;
}
