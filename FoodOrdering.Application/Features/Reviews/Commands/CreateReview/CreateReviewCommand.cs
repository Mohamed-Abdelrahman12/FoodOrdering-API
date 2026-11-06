using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Reviews.DTOs;
using MediatR;


namespace FoodOrdering.Application.Features.Reviews.Commands.CreateReview
{
    public record CreateReviewCommand(
          string UserId,
          Guid ProductId,
          Guid? OrderId,
          int Rating,
          string? Comment
      ) : IRequest<ApiResponse<ReviewDto>>;
}
