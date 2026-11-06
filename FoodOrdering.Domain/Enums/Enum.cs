
namespace FoodOrdering.Domain.Enums
{
    public enum OrderStatus
    {
        Pending = 1,
        Confirmed = 2,
        Preparing = 3,
        OutForDelivery = 4,
        Delivered = 5,
        Cancelled = 6
    }

    public enum PaymentMethod
    {
        Online = 1,
        CashOnDelivery = 2
    }

    public enum PaymentStatus
    {
        Pending = 1,
        Paid = 2,
        Failed = 3,
        Refunded = 4
    }

    public enum UserRole
    {
        Customer = 1,
        Admin = 2
    }
}
