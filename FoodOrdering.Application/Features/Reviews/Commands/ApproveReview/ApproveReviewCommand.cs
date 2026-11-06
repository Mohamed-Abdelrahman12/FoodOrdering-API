using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Reviews.DTOs;
using MediatR;

namespace FoodOrdering.Application.Features.Reviews.Commands.ApproveReview
{
    public record ApproveReviewCommand(Guid ReviewId)
      : IRequest<ApiResponse<ReviewDto>>;
}
