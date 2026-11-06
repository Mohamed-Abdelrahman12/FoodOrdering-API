using FoodOrdering.Domain.Common;

namespace FoodOrdering.Domain.Entities
{
    public class Cart : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;

        public ApplicationUser User { get; set; } = null!;
        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();

        public decimal TotalAmount => Items.Sum(i => i.TotalPrice);
    }

    public class CartItem : BaseEntity
    {
        public Guid CartId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }

        public Cart Cart { get; set; } = null!;
        public Product Product { get; set; } = null!;

        public decimal TotalPrice => Quantity * (Product?.Price ?? 0);
    }

}
