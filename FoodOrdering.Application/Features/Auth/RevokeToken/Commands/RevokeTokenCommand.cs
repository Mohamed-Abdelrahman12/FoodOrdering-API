using FoodOrdering.Application.Common.Models;
using MediatR;

namespace FoodOrdering.Application.Features.Auth.RevokeToken.Commands
{
    public record RevokeTokenCommand(string RefreshToken) : IRequest<ApiResponse<bool>>;

}
