using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Contacts.DTOs;
using MediatR;

namespace FoodOrdering.Application.Features.Contacts.Commands.MarkAsRead
{
    public record MarkAsReadCommand(
     Guid ContactId,
     bool IsReplied = false,
     string? AdminNotes = null
 ) : IRequest<ApiResponse<ContactDto>>;
}
