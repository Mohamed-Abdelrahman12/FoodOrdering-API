using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Categories.DTOs;
using MediatR;
namespace FoodOrdering.Application.Features.Categories.Queries.GetAllCategories
{
    public record GetAllCategoriesQuery(bool ActiveOnly = true) : IRequest<ApiResponse<IEnumerable<CategoryDto>>>;
}
