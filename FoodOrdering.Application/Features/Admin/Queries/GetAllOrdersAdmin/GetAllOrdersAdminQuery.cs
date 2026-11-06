using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Orders.DTOs;
using FoodOrdering.Domain.Enums;
using MediatR;


namespace FoodOrdering.Application.Features.Admin.Queries.GetAllOrdersAdmin
{
    public record GetAllOrdersAdminQuery(int Page = 1, int PageSize = 20, OrderStatus? StatusFilter = null)
      : IRequest<ApiResponse<PagedResult<OrderDto>>>;
}
