using FoodOrdering.Application.Features.Admin.Commands.ToggleUserStatus;
using FoodOrdering.Application.Features.Admin.Queries.GetAllOrdersAdmin;
using FoodOrdering.Application.Features.Admin.Queries.GetAllUsersAdmin;
using FoodOrdering.Application.Features.Admin.Queries.GetDashboardStats;
using FoodOrdering.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrdering.API.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : BaseController
    {
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var result = await Mediator.Send(new GetDashboardStatsQuery());
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetAllOrders(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] OrderStatus? status = null)
        {
            var result = await Mediator.Send(new GetAllOrdersAdminQuery(page, pageSize, status));
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await Mediator.Send(new GetAllUsersAdminQuery(page, pageSize));
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("users/{userId}/toggle-status")]
        public async Task<IActionResult> ToggleUserStatus(string userId)
        {
            var result = await Mediator.Send(new ToggleUserStatusCommand(userId));
            return StatusCode(result.StatusCode, result);
        }
    }
}
