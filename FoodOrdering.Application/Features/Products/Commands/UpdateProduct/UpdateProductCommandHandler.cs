using AutoMapper;
using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Products.DTOs;
using FoodOrdering.Domain.Entities;
using FoodOrdering.Domain.Exceptions;
using FoodOrdering.Domain.Interfaces;
using FoodOrdering.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodOrdering.Application.Features.Products.Commands.UpdateProduct
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ApiResponse<ProductDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IImageManagementService _imageService;
        public UpdateProductCommandHandler(IUnitOfWork uow, IMapper mapper, IImageManagementService imageService)
        {
            _uow = uow; _mapper = mapper;
            _imageService = imageService;
        }

        public async Task<ApiResponse<ProductDto>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _uow.Products.Query()
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Product), request.Id);

            if (product.CategoryId != request.CategoryId)
            {
                var categoryExists = await _uow.Categories
                    .ExistsAsync(c => c.Id == request.CategoryId, cancellationToken);

                if (!categoryExists)
                    return ApiResponse<ProductDto>.Fail("Category not found.");
            }
            if (request.ImageUrl is not null && request.ImageUrl.Length > 0)
            {
                var newImagePath = await _imageService.AddImageAsync(request.ImageUrl, "Products");

                if (!string.IsNullOrWhiteSpace(product.ImageUrl))
                {
                     _imageService.DeleteImageAsync(product.ImageUrl);
                }

                product.ImageUrl = newImagePath;
            }
            product.NameEn = request.NameEn;
            product.NameAr = request.NameAr;
            product.DescriptionEn = request.DescriptionEn;
            product.DescriptionAr = request.DescriptionAr;
            product.Price = request.Price;
            //product.ImageUrl = request.ImageUrl;
            product.StockQuantity = request.StockQuantity;
            product.IsAvailable = request.IsAvailable;
            product.CategoryId = request.CategoryId;
            //if (request.ImageUrl != null)
            //{
            //    // Delete old image if it exists (Optional/Best Practice)
            //    if (!string.IsNullOrEmpty(product.ImageUrl))
            //    {
            //        _imageService.DeleteImageAsync(product.ImageUrl);
            //    }

            //    // Upload new image
            //    //var files = new FormFileCollection { request.UpdateCategoryDTO.Image };
            //    var path = await _imageService.AddImageAsync(request.ImageUrl, "Products");
            //    product.ImageUrl = path;
            //}
            await _uow.Products.UpdateAsync(product, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);
            var UpdatedProductMapping =  _mapper.Map<ProductDto>(product);
            return ApiResponse<ProductDto>.Ok(UpdatedProductMapping);
        }
    }
}
