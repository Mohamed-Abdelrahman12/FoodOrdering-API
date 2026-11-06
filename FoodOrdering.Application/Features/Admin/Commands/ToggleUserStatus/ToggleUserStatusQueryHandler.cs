using FoodOrdering.Application.Common.Models;
using FoodOrdering.Domain.Entities;
using FoodOrdering.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace FoodOrdering.Application.Features.Admin.Commands.ToggleUserStatus
{
    internal class ToggleUserStatusQueryHandler : IRequestHandler<ToggleUserStatusCommand, ApiResponse<bool>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ToggleUserStatusQueryHandler(UserManager<ApplicationUser> userManager) => _userManager = userManager;

        public async Task<ApiResponse<bool>> Handle(ToggleUserStatusCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId)
                ?? throw new NotFoundException("User", request.UserId);

            user.IsActive = !user.IsActive;
            await _userManager.UpdateAsync(user);

            return ApiResponse<bool>.Ok(user.IsActive, user.IsActive ? "User activated." : "User deactivated.");
        }
    }

}
