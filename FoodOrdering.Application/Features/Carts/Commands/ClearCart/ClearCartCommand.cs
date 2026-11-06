using FoodOrdering.Application.Common.Models;
using MediatR;

namespace FoodOrdering.Application.Features.Carts.Commands.ClearCart
{
    public record ClearCartCommand(string UserId) : IRequest<ApiResponse<bool>>;

}
