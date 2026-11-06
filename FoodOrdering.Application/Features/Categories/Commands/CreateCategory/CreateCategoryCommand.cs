using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Categories.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;
namespace FoodOrdering.Application.Features.Categories.Commands.CreateCategory
{
    public record CreateCategoryCommand(string NameEn, string NameAr, IFormFile? ImageUrl) : IRequest<ApiResponse<CategoryDto>>;
}
