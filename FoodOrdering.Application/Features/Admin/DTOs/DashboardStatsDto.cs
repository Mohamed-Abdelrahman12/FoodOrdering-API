
namespace FoodOrdering.Application.Features.Admin.DTOs
{
    public class DashboardStatsDto
    {
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int DeliveredOrders { get; set; }
        public int CancelledOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int TotalCustomers { get; set; }
        public List<RecentOrderDto> RecentOrders { get; set; } = new();
    }

    public class RecentOrderDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

}
