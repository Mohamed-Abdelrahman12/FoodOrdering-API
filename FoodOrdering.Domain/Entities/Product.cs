using FoodOrdering.Domain.Common;

namespace FoodOrdering.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string NameEn { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string DescriptionEn { get; set; } = string.Empty;
        public string DescriptionAr { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsAvailable { get; set; } = true;
        public int StockQuantity { get; set; }
        public Guid CategoryId { get; set; }

        public Category Category { get; set; } = null!;
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();

        public double AverageRating =>
            Reviews.Any(r => r.IsApproved)
                ? Reviews.Where(r => r.IsApproved).Average(r => r.Rating)
                : 0;
    }
}
