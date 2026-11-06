using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Orders.DTOs;
using FoodOrdering.Domain.Enums;
using MediatR;

namespace FoodOrdering.Application.Features.Orders.Commands.PlaceOrder
{
    public record PlaceOrderCommand(
    string UserId,
    string DeliveryAddress,
    PaymentMethod PaymentMethod,
    string? Notes
) : IRequest<ApiResponse<OrderDto>>;

}
