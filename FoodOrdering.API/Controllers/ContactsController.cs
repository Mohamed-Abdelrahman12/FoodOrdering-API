using FoodOrdering.Application.Features.Contacts.Commands.CreateContact;
using FoodOrdering.Application.Features.Contacts.Commands.DeleteContact;
using FoodOrdering.Application.Features.Contacts.Commands.MarkAsRead;
using FoodOrdering.Application.Features.Contacts.Queries.GetAllContacts;
using FoodOrdering.Application.Features.Contacts.Queries.GetContactById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrdering.API.Controllers
{
    public class ContactsController : BaseController
    {
    
        [HttpPost]
        public async Task<IActionResult> Submit([FromBody] CreateContactCommand command)
        {
            var result = await Mediator.Send(command);
            return StatusCode(result.StatusCode, result);
        }

       
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll(
            [FromQuery] bool? isRead = null,
            [FromQuery] bool? isReplied = null,
             [FromQuery] DateOnly? StartDate = null,
             [FromQuery] DateOnly? EndDate = null,
               [FromQuery] string? Email = null,
              [FromQuery] string? FullNameSearchTerm = null,
             [FromQuery] string? MessageContent = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await Mediator.Send(
                new GetAllContactsQuery(isRead, isReplied, StartDate, EndDate, Email, FullNameSearchTerm, MessageContent, page, pageSize));
            return StatusCode(result.StatusCode, result);
        }

      
        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await Mediator.Send(new GetContactByIdQuery(id));
            return StatusCode(result.StatusCode, result);
        }

    
        [HttpPatch("{id:guid}/mark")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Mark(Guid id, [FromBody] MarkContactRequest request)
        {
            var result = await Mediator.Send(
                new MarkAsReadCommand(id, request.IsReplied, request.AdminNotes));
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await Mediator.Send(new DeleteContactCommand(id));
            return StatusCode(result.StatusCode, result);
        }
    }
    public record MarkContactRequest(
        bool IsReplied = false,
        string? AdminNotes = null);
}
