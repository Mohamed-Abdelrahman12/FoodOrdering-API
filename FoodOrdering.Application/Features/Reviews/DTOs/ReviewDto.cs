

namespace FoodOrdering.Application.Features.Reviews.DTOs
{
    public class ReviewDto
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserFullName { get; set; } = string.Empty;
        public Guid ProductId { get; set; }
        public string ProductNameEn { get; set; } = string.Empty;
        public string ProductNameAr { get; set; } = string.Empty;
        public Guid? OrderId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public bool IsApproved { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ProductReviewSummaryDto
    {
        public Guid ProductId { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public Dictionary<int, int> RatingDistribution { get; set; } = new();
        public IEnumerable<ReviewDto> Reviews { get; set; } = new List<ReviewDto>();
    }

   
}
