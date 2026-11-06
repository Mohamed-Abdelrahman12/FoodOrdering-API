
using AutoMapper;
using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Products.DTOs;
using FoodOrdering.Domain.Entities;
using FoodOrdering.Domain.Interfaces;
using FoodOrdering.Infrastructure.Services;
using MediatR;
namespace FoodOrdering.Application.Features.Products.Commands.CreateProduct
{
    internal class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ApiResponse<ProductDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IImageManagementService _imageService;
        public CreateProductCommandHandler(IUnitOfWork uow, IMapper mapper, IImageManagementService imageService)
        {
            _uow = uow; _mapper = mapper;
            _imageService = imageService;
        }

        public async Task<ApiResponse<ProductDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var categoryExists = await _uow.Categories.ExistsAsync(c => c.Id == request.CategoryId, cancellationToken);
            if (!categoryExists)
                return ApiResponse<ProductDto>.Fail("Category not found.");

            string? imagePath = null;
            if (request.ImageUrl is not null && request.ImageUrl.Length > 0)
            {
                imagePath = await _imageService.AddImageAsync(request.ImageUrl, "Products");
            }

            var product = new Product
            {
                NameEn = request.NameEn,
                NameAr = request.NameAr,
                DescriptionEn = request.DescriptionEn,
                DescriptionAr = request.DescriptionAr,
                Price = request.Price,
                ImageUrl = imagePath,
                StockQuantity = request.StockQuantity,
                CategoryId = request.CategoryId
            };

            await _uow.Products.AddAsync(product, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            //var created = await _uow.Products.Query()
            //    .Include(p => p.Category)
            //    .FirstOrDefaultAsync(p => p.Id == product.Id, cancellationToken);
            var createdProductMaping = _mapper.Map<ProductDto>(product);
            return ApiResponse<ProductDto>.Created(createdProductMaping);
        }
    }
}
