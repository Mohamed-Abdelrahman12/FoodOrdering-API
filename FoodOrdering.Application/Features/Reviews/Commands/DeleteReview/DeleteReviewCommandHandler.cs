using FoodOrdering.Application.Common.Models;
using FoodOrdering.Domain.Entities;
using FoodOrdering.Domain.Exceptions;
using FoodOrdering.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace FoodOrdering.Application.Features.Reviews.Commands.DeleteReview
{
    public class DeleteReviewCommandHandler
      : IRequestHandler<DeleteReviewCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _uow;

        public DeleteReviewCommandHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<ApiResponse<bool>> Handle(
            DeleteReviewCommand request,
            CancellationToken cancellationToken)
        {
            // Build query — include User and Product for relation integrity checks
            var query = _uow.Reviews
                .Query()
                .Include(r => r.User)
                .Include(r => r.Product)
                .Where(r => r.Id == request.ReviewId);

            // Non-admin users can only delete reviews they own
            if (!request.IsAdmin)
                query = query.Where(r => r.UserId == request.UserId);

            var review = await query.FirstOrDefaultAsync(cancellationToken)
                ?? throw new NotFoundException(nameof(Review), request.ReviewId);

            // Soft delete — keeps the record for audit purposes
            review.IsDeleted = true;
            review.UpdatedAt = DateTime.UtcNow;

            await _uow.Reviews.UpdateAsync(review, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.Ok(true, "Review deleted successfully.");
        }
    }
}
