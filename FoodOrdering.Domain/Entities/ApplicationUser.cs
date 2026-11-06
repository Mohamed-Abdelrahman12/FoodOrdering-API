using Microsoft.AspNetCore.Identity;


namespace FoodOrdering.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string PreferredLanguage { get; set; } = "en";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public Cart? Cart { get; set; }
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
