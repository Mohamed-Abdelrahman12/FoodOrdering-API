using AutoMapper;
using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Categories.DTOs;
using FoodOrdering.Domain.Entities;
using FoodOrdering.Domain.Interfaces;
using FoodOrdering.Infrastructure.Caching;
using FoodOrdering.Infrastructure.Services;
using MediatR;

namespace FoodOrdering.Application.Features.Categories.Commands.CreateCategory
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, ApiResponse<CategoryDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;
        private readonly IImageManagementService _imageService;

        public CreateCategoryCommandHandler(IUnitOfWork uow, IMapper mapper, ICacheService cache, IImageManagementService imageService)
        {
            _uow = uow;
            _mapper = mapper;
            _cache = cache;
            _imageService = imageService;
        }

        public async Task<ApiResponse<CategoryDto>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var imagePaths = await _imageService.AddImageAsync(request.ImageUrl!, "categories");
            var category = new Category
            {
                NameEn = request.NameEn,
                NameAr = request.NameAr,
                ImageUrl = imagePaths
            };

            await _uow.Categories.AddAsync(category, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            _cache.Remove("categories_true");
            _cache.Remove("categories_false");

            return ApiResponse<CategoryDto>.Created(_mapper.Map<CategoryDto>(category));
        }
    }
}
