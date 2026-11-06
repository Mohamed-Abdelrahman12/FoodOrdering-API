

namespace FoodOrdering.Application.Features.Contacts.DTOs
{
    public class ContactDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public bool IsReplied { get; set; }
        public DateTime? RepliedAt { get; set; }
        public string? AdminNotes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
