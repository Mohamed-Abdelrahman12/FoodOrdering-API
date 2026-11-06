using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Orders.DTOs;
using MediatR;

namespace FoodOrdering.Application.Features.Orders.Queries.GetUserOrders
{
    public record GetUserOrdersQuery(string UserId, int Page = 1, int PageSize = 10)
     : IRequest<ApiResponse<PagedResult<OrderDto>>>;

}
