using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Contacts.DTOs;
using MediatR;


namespace FoodOrdering.Application.Features.Contacts.Queries.GetContactById
{
    public record GetContactByIdQuery(Guid ContactId)
      : IRequest<ApiResponse<ContactDto>>;
}
