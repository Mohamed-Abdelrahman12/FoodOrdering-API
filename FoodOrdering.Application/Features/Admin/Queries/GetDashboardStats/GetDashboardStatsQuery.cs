using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Admin.DTOs;
using MediatR;


namespace FoodOrdering.Application.Features.Admin.Queries.GetDashboardStats
{
    public record GetDashboardStatsQuery : IRequest<ApiResponse<DashboardStatsDto>>;

}
