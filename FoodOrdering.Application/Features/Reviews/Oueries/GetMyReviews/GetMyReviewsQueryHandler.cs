using AutoMapper;
using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Reviews.DTOs;
using FoodOrdering.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodOrdering.Application.Features.Reviews.Oueries.GetMyReviews
{
    public class GetMyReviewsQueryHandler
    : IRequestHandler<GetMyReviewsQuery, ApiResponse<PagedResult<ReviewDto>>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetMyReviewsQueryHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<ApiResponse<PagedResult<ReviewDto>>> Handle(
            GetMyReviewsQuery request,
            CancellationToken cancellationToken)
        {
            var query = _uow.Reviews
                .Query()
                .Include(r => r.User)
                .Include(r => r.Product)
                .Where(r => r.UserId == request.UserId)
                .OrderByDescending(r => r.CreatedAt);

            var totalCount = await query.CountAsync(cancellationToken);

            var reviews = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return ApiResponse<PagedResult<ReviewDto>>.Ok(new PagedResult<ReviewDto>
            {
                Items = _mapper.Map<IEnumerable<ReviewDto>>(reviews),
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            });
        }
    }
}
