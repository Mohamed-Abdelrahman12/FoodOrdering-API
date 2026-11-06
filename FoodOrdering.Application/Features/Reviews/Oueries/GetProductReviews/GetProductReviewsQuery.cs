using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Reviews.DTOs;
using MediatR;

namespace FoodOrdering.Application.Features.Reviews.Oueries.GetProductReviews
{
    public record GetProductReviewsQuery(
    Guid ProductId,
    int Page = 1,
    int PageSize = 10
) : IRequest<ApiResponse<ProductReviewSummaryDto>>;
}
