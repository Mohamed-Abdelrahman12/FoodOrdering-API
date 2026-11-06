

using FoodOrdering.Application.Common.Models;
using MediatR;

namespace FoodOrdering.Application.Features.Reviews.Commands.DeleteReview
{
    public record DeleteReviewCommand(
     Guid ReviewId,
     string UserId,
     bool IsAdmin = false
 ) : IRequest<ApiResponse<bool>>;
}
