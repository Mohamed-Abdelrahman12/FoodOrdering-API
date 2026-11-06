using FoodOrdering.Domain.Entities;
using System.Security.Claims;


namespace FoodOrdering.Infrastructure.Services
{
    public interface ITokenService
    {
        Task<string> GenerateAccessTokenAsync(ApplicationUser user);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
