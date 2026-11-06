using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Contacts.DTOs;
using MediatR;


namespace FoodOrdering.Application.Features.Contacts.Queries.GetAllContacts
{
    public record GetAllContactsQuery(
     bool? IsRead = null,
     bool? IsReplied = null,
     DateOnly? StartDate = null,
     DateOnly? EndDate = null,
     string? Email = null,
     string? FullNameSearchTerm = null,
     string? MessageContent = null,
     int Page = 1,
     int PageSize = 20
 ) : IRequest<ApiResponse<PagedResult<ContactDto>>>;
}
