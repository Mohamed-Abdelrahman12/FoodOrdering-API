using AutoMapper;
using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Categories.DTOs;
using FoodOrdering.Domain.Entities;
using FoodOrdering.Domain.Interfaces;
using FoodOrdering.Infrastructure.Caching;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace FoodOrdering.Application.Features.Categories.Queries.GetAllCategories
{
    public record GetAllCategoriesQueryHandler(bool ActiveOnly = true) : IRequest<ApiResponse<IEnumerable<CategoryDto>>>;

    public class GetAllCategoriesHandler : IRequestHandler<GetAllCategoriesQuery, ApiResponse<IEnumerable<CategoryDto>>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;

        public GetAllCategoriesHandler(IUnitOfWork uow, IMapper mapper, ICacheService cache)
        {
            _uow = uow;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<ApiResponse<IEnumerable<CategoryDto>>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            //var cacheKey = $"categories_{request.ActiveOnly}";
            //if (_cache.TryGet<IEnumerable<CategoryDto>>(cacheKey, out var cached))
            //    return ApiResponse<IEnumerable<CategoryDto>>.Ok(cached!);
            var query = _uow.Categories.Query().Include(p => p.Products).AsQueryable();

            if (request.ActiveOnly)
                query = query.Where(p => p.IsActive);
            //IQueryable<Category> query = _uow.Categories.Query().Include(c => c.Products);
            //if (request.ActiveOnly)
            //    query = query.Where(c => c.IsActive);

            var categories = await query.ToListAsync(cancellationToken);
            var dtos = categories.Select(c =>
            {
                var dto = _mapper.Map<CategoryDto>(c);
                dto.ProductCount = c.Products.Count(p => !p.IsDeleted);
                return dto;
            });

            //_cache.Set(cacheKey, dtos, TimeSpan.FromMinutes(10));
            return ApiResponse<IEnumerable<CategoryDto>>.Ok(dtos);
        }
    }


}
