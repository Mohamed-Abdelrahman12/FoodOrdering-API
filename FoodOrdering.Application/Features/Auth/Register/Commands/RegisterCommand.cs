using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Auth.DTOs;
using MediatR;
namespace FoodOrdering.Application.Features.Auth.Register.Commands
{
    public record RegisterCommand(
     string FirstName,
     string LastName,
     string Email,
     string Password,
     string? PhoneNumber,
     string? Address,
     string PreferredLanguage = "en"
 ) : IRequest<ApiResponse<UserDto>>;

}
