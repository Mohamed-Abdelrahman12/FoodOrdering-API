using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Orders.DTOs;
using MediatR;

namespace FoodOrdering.Application.Features.Orders.Queries.GetOrderById
{
    public record GetOrderByIdQuery(Guid OrderId, string UserId, bool IsAdmin = false)
     : IRequest<ApiResponse<OrderDto>>;

}
