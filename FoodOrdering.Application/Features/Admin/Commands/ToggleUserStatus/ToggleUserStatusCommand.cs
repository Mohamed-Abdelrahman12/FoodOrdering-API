using FoodOrdering.Application.Common.Models;
using MediatR;


namespace FoodOrdering.Application.Features.Admin.Commands.ToggleUserStatus
{
    public record ToggleUserStatusCommand(string UserId) : IRequest<ApiResponse<bool>>;
}
