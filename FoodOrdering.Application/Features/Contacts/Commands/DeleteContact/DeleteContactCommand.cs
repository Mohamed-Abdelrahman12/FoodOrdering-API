using FoodOrdering.Application.Common.Models;
using MediatR;


namespace FoodOrdering.Application.Features.Contacts.Commands.DeleteContact
{
    public record DeleteContactCommand(Guid ContactId) : IRequest<ApiResponse<bool>>;
}
