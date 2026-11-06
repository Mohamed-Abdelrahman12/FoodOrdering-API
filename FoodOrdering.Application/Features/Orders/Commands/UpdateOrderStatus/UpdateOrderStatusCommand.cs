using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Orders.DTOs;
using FoodOrdering.Domain.Enums;
using MediatR;


namespace FoodOrdering.Application.Features.Orders.Commands.UpdateOrderStatus
{
    public record UpdateOrderStatusCommand(Guid OrderId, OrderStatus NewStatus, string AdminUserId)
        : IRequest<ApiResponse<OrderDto>>;
}
