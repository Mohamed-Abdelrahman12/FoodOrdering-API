using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Products.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FoodOrdering.Application.Features.Products.Commands.CreateProduct
{
    //    public record CreateProductCommand(
    //    string NameEn, string NameAr,
    //    string DescriptionEn, string DescriptionAr,
    //    decimal Price, IFormFile? ImageUrl,
    //    int StockQuantity, Guid CategoryId
    //) : IRequest<ApiResponse<ProductDto>>;
    public class CreateProductCommand : IRequest<ApiResponse<ProductDto>>
    {
        public string NameEn { get; init; } = string.Empty;
        public string NameAr { get; init; } = string.Empty;
        public string DescriptionEn { get; init; } = string.Empty;
        public string DescriptionAr { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public int StockQuantity { get; init; }
        public Guid CategoryId { get; init; }
        public IFormFile? ImageUrl { get; init; }
    }

}
