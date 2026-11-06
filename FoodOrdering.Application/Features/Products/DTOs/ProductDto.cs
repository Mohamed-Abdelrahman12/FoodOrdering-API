
using Microsoft.AspNetCore.Http;

namespace FoodOrdering.Application.Features.Products.DTOs
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string NameEn { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string DescriptionEn { get; set; } = string.Empty;
        public string DescriptionAr { get; set; } = string.Empty;
        public decimal Price { get; set; }
        //public IFormFile? ImageUrl { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsAvailable { get; set; }
        public int StockQuantity { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryNameEn { get; set; } = string.Empty;
        public string CategoryNameAr { get; set; } = string.Empty;
        // Aggregated from approved Reviews
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }
    public class ReturnProductDto
    {
        public Guid Id { get; set; }
        public string NameEn { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string DescriptionEn { get; set; } = string.Empty;
        public string DescriptionAr { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsAvailable { get; set; }
        public int StockQuantity { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryNameEn { get; set; } = string.Empty;
        public string CategoryNameAr { get; set; } = string.Empty;
        // Aggregated from approved Reviews
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }
}
