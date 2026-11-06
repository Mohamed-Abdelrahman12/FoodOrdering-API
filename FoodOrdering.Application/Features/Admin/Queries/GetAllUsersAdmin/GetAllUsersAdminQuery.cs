using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Auth.DTOs;
using MediatR;


namespace FoodOrdering.Application.Features.Admin.Queries.GetAllUsersAdmin
{
    public record GetAllUsersAdminQuery(int Page = 1, int PageSize = 20) : IRequest<ApiResponse<PagedResult<UserDto>>>;
}
