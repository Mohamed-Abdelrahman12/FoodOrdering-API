

namespace FoodOrdering.Application.Features.Categories.DTOs
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string NameEn { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public int ProductCount { get; set; }
    }
}
