using AutoMapper;
using FoodOrdering.Application.Common.Filtering;
using FoodOrdering.Application.Common.Filtering.Rules;
using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Reviews.DTOs;
using FoodOrdering.Domain.Entities;
using FoodOrdering.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace FoodOrdering.Application.Features.Reviews.Oueries.GetAllReviews
{
    public class GetAllReviewsQueryHandler
      : IRequestHandler<GetAllReviewsQuery, ApiResponse<PagedResult<ReviewDto>>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetAllReviewsQueryHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<ApiResponse<PagedResult<ReviewDto>>> Handle(
            GetAllReviewsQuery request,
            CancellationToken cancellationToken)
        {
            var query = _uow.Reviews
                .Query()
                .Include(r => r.User)
                .Include(r => r.Product)
                .AsQueryable();

            IFilterRule<Review>[] rules =
          {
                new BooleanFilterRule<Review>(
                    r => r.IsApproved,
                    request.IsApproved),

                new EqualityFilterRule<Review, int>(
                    r => r.Rating,
                    request.Rating),

                new DateRangeFilterRule<Review>(
                    r => r.CreatedAt,
                    request.StartDate,
                    request.EndDate),

                new KeywordSearchFilterRule<Review>(
                    request.ProductSearchTerm,
                    word => r => r.Product.NameAr.ToLower().Contains(word) ||
                                 r.Product.NameEn.ToLower().Contains(word)),

                new KeywordSearchFilterRule<Review>(
                    request.FullNameSearchTerm,
                    word => r => r.User.FirstName.ToLower().Contains(word) ||
                                 r.User.LastName.ToLower().Contains(word)),
            };

            foreach (var rule in rules.Where(r => r.IsApplicable()))
                query = rule.Apply(query);

            var paged = await PaginationHelper.ToPagedResultAsync(
               query,
               request.Page,
               request.PageSize,
               entities => _mapper.Map<IEnumerable<ReviewDto>>(entities),
               q => q.OrderByDescending(r => r.CreatedAt),
               cancellationToken);

            return ApiResponse<PagedResult<ReviewDto>>.Ok(paged);

          
        }
    }
}
