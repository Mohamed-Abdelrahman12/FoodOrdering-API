using FoodOrdering.Domain.Enums;

namespace FoodOrdering.Application.Features.Orders.DTOs
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string DeliveryAddress { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public OrderStatus Status { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
        public OrderPaymentDto? Payment { get; set; }
    }

    public class OrderItemDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductNameEn { get; set; } = string.Empty;
        public string ProductNameAr { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class OrderPaymentDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string MethodName { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;
        public PaymentStatus Status { get; set; }
        public string? TransactionId { get; set; }
        public DateTime? PaidAt { get; set; }
    }
}
