using FoodOrdering.Application.Common.Models;
using MediatR;

namespace FoodOrdering.Application.Features.Categories.Commands.DeleteCategory
{
    public record DeleteCategoryCommand(Guid Id) : IRequest<ApiResponse<bool>>;

}
