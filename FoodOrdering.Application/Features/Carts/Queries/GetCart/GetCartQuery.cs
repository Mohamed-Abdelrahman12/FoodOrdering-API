using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Carts.DTOs;
using MediatR;
namespace FoodOrdering.Application.Features.Cart.Queries.GetCart
{
    public record GetCartQuery(string UserId) : IRequest<ApiResponse<CartDto>>;

}
