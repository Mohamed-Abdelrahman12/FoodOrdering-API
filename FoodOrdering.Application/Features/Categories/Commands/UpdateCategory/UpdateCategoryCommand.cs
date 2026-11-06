using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Categories.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;
namespace FoodOrdering.Application.Features.Categories.Commands.UpdateCategory
{
    public record UpdateCategoryCommand(Guid Id, string NameEn, string NameAr, IFormFile? ImageUrl, bool IsActive) : IRequest<ApiResponse<CategoryDto>>;
}
