using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Carts.DTOs;
using MediatR;
namespace FoodOrdering.Application.Features.Carts.Commands.RemoveCartItem
{
    public record RemoveCartItemCommand(string UserId, Guid ProductId)
     : IRequest<ApiResponse<CartDto>>;
}
