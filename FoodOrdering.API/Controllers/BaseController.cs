using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodOrdering.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public abstract class BaseController : ControllerBase
    {
        private ISender? _mediator;
        protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

        protected string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        protected string CurrentUserEmail => User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
        protected bool IsAdmin => User.IsInRole("Admin");
        protected string CurrentLanguage => HttpContext.Items["Language"]?.ToString() ?? "en";
    }
}
