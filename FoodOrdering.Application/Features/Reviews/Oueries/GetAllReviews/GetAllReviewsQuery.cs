using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Reviews.DTOs;
using MediatR;

namespace FoodOrdering.Application.Features.Reviews.Oueries.GetAllReviews
{
    public record GetAllReviewsQuery(
       bool? IsApproved = null,
       int Page = 1,
       int PageSize = 20,
       int? Rating = null,
       string? ProductSearchTerm = null,
       string? FullNameSearchTerm = null,
       DateOnly? StartDate = null,
       DateOnly? EndDate = null
   ) : IRequest<ApiResponse<PagedResult<ReviewDto>>>;
}
