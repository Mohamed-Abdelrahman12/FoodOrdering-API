using FoodOrdering.Application.Common.Models;
using MediatR;

namespace FoodOrdering.Application.Features.Orders.Commands.CancelOrder
{
    public record CancelOrderCommand(Guid OrderId, string UserId, bool IsAdmin = false)
    : IRequest<ApiResponse<bool>>;
}
