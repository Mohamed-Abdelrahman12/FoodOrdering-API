using AutoMapper;
using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Reviews.DTOs;
using FoodOrdering.Domain.Entities;
using FoodOrdering.Domain.Enums;
using FoodOrdering.Domain.Exceptions;
using FoodOrdering.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodOrdering.Application.Features.Reviews.Commands.CreateReview
{
    public class CreateReviewCommandHandler
         : IRequestHandler<CreateReviewCommand, ApiResponse<ReviewDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public CreateReviewCommandHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<ApiResponse<ReviewDto>> Handle(
            CreateReviewCommand request,
            CancellationToken cancellationToken)
        {
            // 1. Product must exist
            var productExists = await _uow.Products
                .ExistsAsync(p => p.Id == request.ProductId, cancellationToken);

            if (!productExists)
                throw new NotFoundException(nameof(Product), request.ProductId);

            // 2. If OrderId is provided, verify that the order belongs to the user
            //    and contains the product (prevents fake verified reviews)
            if (request.OrderId.HasValue)
            {
                var validOrder = await _uow.Orders.Query()
                    .Include(o => o.Items)
                    .AnyAsync(o => o.Id == request.OrderId
                               && o.UserId == request.UserId
                               && o.Status == OrderStatus.Delivered
                               && o.Items.Any(i => i.ProductId == request.ProductId),
                        cancellationToken);

                if (!validOrder)
                    return ApiResponse<ReviewDto>.Fail(
                        "You can only leave a verified review for a delivered order that contains this product.");
            }

            // 3. One review per user per product
            var alreadyReviewed = await _uow.Reviews
                .ExistsAsync(r => r.UserId == request.UserId
                               && r.ProductId == request.ProductId,
                    cancellationToken);

            if (alreadyReviewed)
                return ApiResponse<ReviewDto>.Fail(
                    "You have already submitted a review for this product.");

            var review = new Review
            {
                UserId = request.UserId,
                ProductId = request.ProductId,
                OrderId = request.OrderId,
                Rating = request.Rating,
                Comment = request.Comment,
                IsApproved = false  // requires admin approval
            };

            await _uow.Reviews.AddAsync(review, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            // Reload with navigation properties for mapping
            //var saved = await _uow.Reviews.Query()
            //    .Include(r => r.User)
            //    .Include(r => r.Product)
            //    .FirstOrDefaultAsync(r => r.Id == review.Id, cancellationToken);

            return ApiResponse<ReviewDto>.Created(
                _mapper.Map<ReviewDto>(review),
                "Review submitted and awaiting approval.");
        }
    }
}
