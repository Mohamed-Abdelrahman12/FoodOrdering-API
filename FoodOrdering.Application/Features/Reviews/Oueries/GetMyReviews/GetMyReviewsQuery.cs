using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Reviews.DTOs;
using MediatR;


namespace FoodOrdering.Application.Features.Reviews.Oueries.GetMyReviews
{
    public record GetMyReviewsQuery(
    string UserId,
    int Page = 1,
    int PageSize = 10
) : IRequest<ApiResponse<PagedResult<ReviewDto>>>;
}
