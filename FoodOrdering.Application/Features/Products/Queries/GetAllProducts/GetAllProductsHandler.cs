
using AutoMapper;
using FoodOrdering.Application.Common.Filtering;
using FoodOrdering.Application.Common.Filtering.Rules;
using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Products.DTOs;
using FoodOrdering.Domain.Entities;
using FoodOrdering.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodOrdering.Application.Features.Products.Queries.GetAllProducts
{
    public class GetAllProductsHandler : IRequestHandler<GetAllProductsQuery, ApiResponse<PagedResult<ReturnProductDto>>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetAllProductsHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<ApiResponse<PagedResult<ReturnProductDto>>> Handle(
            GetAllProductsQuery request,
            CancellationToken cancellationToken)
        {
            IQueryable<Product> query = _uow.Products.Query()
                .Include(p => p.Category)
                .Include(p => p.Reviews.Where(r => r.IsApproved))
                .AsQueryable();

            IFilterRule<Product>[] rules =
            {
                
                new BooleanFilterRule<Product>(
                    p => p.IsAvailable,
                    request.AvailableOnly),

                new EqualityFilterRule<Product, Guid>(
                    p => p.CategoryId,
                    request.CategoryId),
                 new EqualityFilterRule<Product, double>(
                    p => p.AverageRating,
                    request.Rating),

                new RangeFilterRule<Product, decimal>(
                    p => p.Price,
                    request.MinPrice,
                    request.MaxPrice),

                new KeywordSearchFilterRule<Product>(
                    request.SearchTerm,
                    word => p => p.NameAr.ToLower().Contains(word) ||
                                 p.NameEn.ToLower().Contains(word) ||
                                 p.DescriptionAr.ToLower().Contains(word) ||
                                 p.DescriptionEn.ToLower().Contains(word)),
            };

            foreach (var rule in rules.Where(r => r.IsApplicable()))
                query = rule.Apply(query);

            query = request.SortOrder?.ToLower() == "desc"
                ? query.OrderByDescending(p => p.Price)
                : query.OrderBy(p => p.Price);

            var paged = await PaginationHelper.ToPagedResultAsync(
                query,
                request.Page,
                request.PageSize,
                entities => _mapper.Map<IEnumerable<ReturnProductDto>>(entities),
                q => (IOrderedQueryable<Product>)q, // already sorted above
                cancellationToken);

            return ApiResponse<PagedResult<ReturnProductDto>>.Ok(paged);
        }
    }
}