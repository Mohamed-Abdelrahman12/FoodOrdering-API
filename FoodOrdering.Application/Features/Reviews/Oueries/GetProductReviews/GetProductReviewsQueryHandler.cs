using AutoMapper;
using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Reviews.DTOs;
using FoodOrdering.Domain.Entities;
using FoodOrdering.Domain.Exceptions;
using FoodOrdering.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodOrdering.Application.Features.Reviews.Oueries.GetProductReviews
{
    public class GetProductReviewsQueryHandler
     : IRequestHandler<GetProductReviewsQuery, ApiResponse<ProductReviewSummaryDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetProductReviewsQueryHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<ApiResponse<ProductReviewSummaryDto>> Handle(
            GetProductReviewsQuery request,
            CancellationToken cancellationToken)
        {
            // Verify product exists
            var productExists = await _uow.Products
                .ExistsAsync(p => p.Id == request.ProductId, cancellationToken);

            if (!productExists)
                throw new NotFoundException(nameof(Product), request.ProductId);

            // Only approved reviews are visible to the public
            var baseQuery = _uow.Reviews
                .Query()
                .Include(r => r.User)
                .Include(r => r.Product)
                .Where(r => r.ProductId == request.ProductId && r.IsApproved);

            var totalCount = await baseQuery.CountAsync(cancellationToken);
            var averageRating = totalCount > 0
                ? await baseQuery.AverageAsync(r => (double)r.Rating, cancellationToken)
                : 0;

            // Rating distribution: { 1: 3, 2: 5, 3: 10, 4: 20, 5: 15 }
            var distribution = await baseQuery
                .GroupBy(r => r.Rating)
                .Select(g => new { Star = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Star, x => x.Count, cancellationToken);

            // Fill missing star levels with 0
            for (int star = 1; star <= 5; star++)
                distribution.TryAdd(star, 0);

            // Paged reviews
            var reviews = await baseQuery
                .OrderByDescending(r => r.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var summary = new ProductReviewSummaryDto
            {
                ProductId = request.ProductId,
                AverageRating = Math.Round(averageRating, 2),
                TotalReviews = totalCount,
                RatingDistribution = distribution,
                Reviews = _mapper.Map<IEnumerable<ReviewDto>>(reviews)
            };

            return ApiResponse<ProductReviewSummaryDto>.Ok(summary);
        }
    }
}