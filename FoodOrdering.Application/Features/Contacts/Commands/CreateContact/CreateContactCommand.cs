using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Contacts.DTOs;
using MediatR;


namespace FoodOrdering.Application.Features.Contacts.Commands.CreateContact
{
    public record CreateContactCommand(
     string FullName,
    string Email,
    string? PhoneNumber,
    string Subject,
    string Message
) : IRequest<ApiResponse<ContactDto>>;
}
