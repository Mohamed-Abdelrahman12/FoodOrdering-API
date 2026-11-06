

using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Categories.DTOs;
using MediatR;

namespace FoodOrdering.Application.Features.Categories.Queries.GetCategoryById
{
    public record GetCategoryByIdQuery(Guid Id) : IRequest<ApiResponse<CategoryDto>>;

}
