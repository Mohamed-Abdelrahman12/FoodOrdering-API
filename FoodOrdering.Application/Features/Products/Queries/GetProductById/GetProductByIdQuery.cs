using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Products.DTOs;
using MediatR;
namespace FoodOrdering.Application.Features.Products.Queries.GetProductById
{
    public record GetProductByIdQuery(Guid Id) : IRequest<ApiResponse<ReturnProductDto>>;

}
