using FoodOrdering.Domain.Common;

namespace FoodOrdering.Domain.Entities
{
    public class Contact : BaseEntity
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        // Admin workflow flags
        public bool IsRead { get; set; } = false;
        public bool IsReplied { get; set; } = false;
        public DateTime? RepliedAt { get; set; }
        public string? AdminNotes { get; set; }
    }
}
