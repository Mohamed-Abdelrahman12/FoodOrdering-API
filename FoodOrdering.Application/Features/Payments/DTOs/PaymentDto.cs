using FoodOrdering.Domain.Enums;

namespace FoodOrdering.Application.Features.Payments.DTOs
{
    public class PaymentDto
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod Method { get; set; }
        public string MethodName { get; set; } = string.Empty;
        public PaymentStatus Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string? TransactionId { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class InitiateOnlinePaymentDto
    {
        public Guid PaymentId { get; set; }
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public string Status { get; set; } = string.Empty;
        public string? PaymentReference { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class CodPaymentDto
    {
        public Guid PaymentId { get; set; }
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Message { get; set; } = string.Empty;
        public string ExpectedAt { get; set; } = string.Empty;
    }
}
