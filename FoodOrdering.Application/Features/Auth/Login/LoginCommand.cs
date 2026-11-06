using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Auth.DTOs;
using MediatR;

namespace FoodOrdering.Application.Features.Auth.Login
{
    public record LoginCommand(string Email, string Password) : IRequest<ApiResponse<AuthResultDto>>;

}
