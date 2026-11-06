using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Carts.DTOs;
using MediatR;


namespace FoodOrdering.Application.Features.Carts.Commands.AddToCart
{

    public record AddToCartCommand(string UserId, Guid ProductId, int Quantity)
        : IRequest<ApiResponse<CartDto>>;
}
