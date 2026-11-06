using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Payments.DTOs;
using MediatR;


namespace FoodOrdering.Application.Features.Payments.Queries.GetPaymentByOrder
{
    public record GetPaymentByOrderQuery(Guid OrderId, string UserId, bool IsAdmin = false)
     : IRequest<ApiResponse<PaymentDto>>;
}
