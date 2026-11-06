using AutoMapper;
using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Reviews.DTOs;
using FoodOrdering.Domain.Entities;
using FoodOrdering.Domain.Exceptions;
using FoodOrdering.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace FoodOrdering.Application.Features.Reviews.Commands.ApproveReview
{
    public class ApproveReviewCommandHandler
      : IRequestHandler<ApproveReviewCommand, ApiResponse<ReviewDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public ApproveReviewCommandHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<ApiResponse<ReviewDto>> Handle(
            ApproveReviewCommand request,
            CancellationToken cancellationToken)
        {
            // Load with all navigation properties needed for ReviewDto mapping:
            //   User    → UserFullName
            //   Product → ProductNameEn / ProductNameAr
            //   Order   → OrderId (nullable relation)
            var review = await _uow.Reviews
                .Query()
                .Include(r => r.User)
                .Include(r => r.Product)
                    .ThenInclude(p => p.Category)   // product needs category for its own DTO
                //.Include(r => r.Order)
                .FirstOrDefaultAsync(r => r.Id == request.ReviewId, cancellationToken)
                ?? throw new NotFoundException(nameof(Review), request.ReviewId);

            if (review.IsApproved)
                return ApiResponse<ReviewDto>.Fail(
                    "This review is already approved.");

            review.IsApproved = true;
            review.UpdatedAt = DateTime.UtcNow;

            await _uow.Reviews.UpdateAsync(review, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return ApiResponse<ReviewDto>.Ok(
                _mapper.Map<ReviewDto>(review),
                "Review approved and is now publicly visible.");
        }
    }
}