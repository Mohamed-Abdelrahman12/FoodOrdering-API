using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Auth.DTOs;
using MediatR;


namespace FoodOrdering.Application.Features.Auth.RefreshTokens.Commands
{

    public record RefreshTokenCommand( string RefreshToken) : IRequest<ApiResponse<AuthResultDto>>;
}
