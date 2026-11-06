
using FoodOrdering.Application.Features.Auth.Login;
using FoodOrdering.Application.Features.Auth.RefreshTokens.Commands;
using FoodOrdering.Application.Features.Auth.Register.Commands;
using FoodOrdering.Application.Features.Auth.RevokeToken.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrdering.API.Controllers
{
    public class AuthController : BaseController
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command)
        {
            var result = await Mediator.Send(command);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var result = await Mediator.Send(command);

            if (!result.Success || result.Data is null)
                return StatusCode(result.StatusCode, result);

            var isHttps = Request.IsHttps;

            AppendAccessTokenCookie(result.Data.AccessToken, result.Data.AccessTokenExpiry);
            AppendRefreshTokenCookie(result.Data.RefreshToken);

            return StatusCode(result.StatusCode, new
            {
                result.Success,
                result.Message,
                result.StatusCode,
                Data = new
                {
                    result.Data.AccessToken,
                    result.Data.AccessTokenExpiry,
                    result.Data.User
                }
            });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest(new { Success = false, Message = "Refresh token cookie is missing." });

            var result = await Mediator.Send(new RefreshTokenCommand(refreshToken));

            if (!result.Success || result.Data is null)
                return StatusCode(result.StatusCode, result);

            AppendAccessTokenCookie(result.Data.AccessToken, result.Data.AccessTokenExpiry);
            AppendRefreshTokenCookie(result.Data.RefreshToken);

            return StatusCode(result.StatusCode, new
            {
                result.Success,
                result.Message,
                result.StatusCode,
                Data = new
                {
                    result.Data.AccessToken,
                    result.Data.AccessTokenExpiry,
                    result.Data.User
                }
            });
        }

        [HttpPost("revoke-token")]
        [Authorize]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenRequest? body)
        {
            var tokenToRevoke = body?.RefreshToken ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(tokenToRevoke))
                return BadRequest(new { Success = false, Message = "Token is required." });

            DeleteAuthCookies();

            var result = await Mediator.Send(new RevokeTokenCommand(tokenToRevoke));
            return StatusCode(result.StatusCode, result);
        }


        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"] ?? string.Empty;
            DeleteAuthCookies();

            if (!string.IsNullOrEmpty(refreshToken))
                await Mediator.Send(new RevokeTokenCommand(refreshToken));

            return Ok(new { Success = true, Message = "Logged out successfully." });
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult Me() => Ok(new
        {
            UserId = CurrentUserId,
            Email = CurrentUserEmail,
            IsAdmin,
            Language = CurrentLanguage
        });


        private void AppendAccessTokenCookie(string token, DateTime expiry )
        {
            Response.Cookies.Append("accessToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,          
                SameSite = SameSiteMode.Strict,
                Expires = new DateTimeOffset(expiry)
            });
        }


        private void AppendRefreshTokenCookie(string token)
        {
            var expiryDays = 7;
            Response.Cookies.Append("refreshToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(expiryDays)
            });
        }


        private void DeleteAuthCookies()
        {
            Response.Cookies.Delete("accessToken");
            Response.Cookies.Delete("refreshToken");
        }
    }

   
    public record RevokeTokenRequest(string? RefreshToken);
}