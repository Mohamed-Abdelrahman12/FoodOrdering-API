using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Carts.DTOs;
using MediatR;


namespace FoodOrdering.Application.Features.Carts.Commands.UpdateCartItem
{
    public record UpdateCartItemCommand(string UserId, Guid ProductId, int Quantity)
        : IRequest<ApiResponse<CartDto>>;
}
