using AutoMapper;
using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Categories.DTOs;
using FoodOrdering.Domain.Entities;
using FoodOrdering.Domain.Exceptions;
using FoodOrdering.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodOrdering.Application.Features.Categories.Queries.GetCategoryById
{
    public class GetCategoryByIdHandler : IRequestHandler<GetCategoryByIdQuery, ApiResponse<CategoryDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetCategoryByIdHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        public async Task<ApiResponse<CategoryDto>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var category = await _uow.Categories.Query()
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Category), request.Id);

            var dto = _mapper.Map<CategoryDto>(category);
            dto.ProductCount = category.Products.Count(p => !p.IsDeleted);
            return ApiResponse<CategoryDto>.Ok(dto);
        }
    }
}
