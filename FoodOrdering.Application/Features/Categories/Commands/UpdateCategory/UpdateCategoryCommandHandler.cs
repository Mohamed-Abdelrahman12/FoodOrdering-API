using AutoMapper;
using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Categories.DTOs;
using FoodOrdering.Domain.Entities;
using FoodOrdering.Domain.Exceptions;
using FoodOrdering.Domain.Interfaces;
using FoodOrdering.Infrastructure.Caching;
using FoodOrdering.Infrastructure.Services;
using MediatR;
namespace FoodOrdering.Application.Features.Categories.Commands.UpdateCategory
{
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, ApiResponse<CategoryDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;
        private readonly IImageManagementService _imageService;
        public UpdateCategoryCommandHandler(IUnitOfWork uow, IMapper mapper, ICacheService cache, IImageManagementService imageService)
        {
            _uow = uow;
            _mapper = mapper;
            _cache = cache;
            _imageService = imageService;
        }

        public async Task<ApiResponse<CategoryDto>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            //var categorys = await _uow.Categories.GetByIdAsync(request.Id, cancellationToken);
            var category = await _uow.Categories.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Category), request.Id);

            category.NameEn = request.NameEn;
            category.NameAr = request.NameAr;
            //category.ImageUrl = request.ImageUrl;
            category.IsActive = request.IsActive;
            if (request.ImageUrl != null)
            {
                // Delete old image if it exists (Optional/Best Practice)
                if (!string.IsNullOrEmpty(category.ImageUrl))
                {
                  _imageService.DeleteImageAsync(category.ImageUrl);
                }

                // Upload new image
                //var files = new FormFileCollection { request.UpdateCategoryDTO.Image };
                var path = await _imageService.AddImageAsync(request.ImageUrl, "categories");
                category.ImageUrl = path;
            }

            await _uow.Categories.UpdateAsync(category, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            _cache.Remove("categories_true");
            _cache.Remove("categories_false");

            return ApiResponse<CategoryDto>.Ok(_mapper.Map<CategoryDto>(category));
        }
    }
}
