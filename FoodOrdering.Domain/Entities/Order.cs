using FoodOrdering.Domain.Common;
using FoodOrdering.Domain.Enums;

namespace FoodOrdering.Domain.Entities
{
    public class Order : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public string DeliveryAddress { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public string? Notes { get; set; }

        public ApplicationUser User { get; set; } = null!;
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
        public Payment? Payment { get; set; }

        //public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }

    public class OrderItem : BaseEntity
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public Order Order { get; set; } = null!;
        public Product Product { get; set; } = null!;

        public decimal TotalPrice => Quantity * UnitPrice;
    }
}
