using FoodOrdering.Application.Common.Models;
using FoodOrdering.Domain.Entities;
using FoodOrdering.Domain.Exceptions;
using FoodOrdering.Domain.Interfaces;
using MediatR;
namespace FoodOrdering.Application.Features.Contacts.Commands.DeleteContact
{
    public class DeleteContactCommandHandler
     : IRequestHandler<DeleteContactCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _uow;

        public DeleteContactCommandHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<ApiResponse<bool>> Handle(
            DeleteContactCommand request,
            CancellationToken cancellationToken)
        {
            var contact = await _uow.Contacts.GetByIdAsync(request.ContactId, cancellationToken)
                ?? throw new NotFoundException(nameof(Contact), request.ContactId);

            contact.IsDeleted = true;

            await _uow.Contacts.UpdateAsync(contact, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.Ok(true, "Contact message deleted.");
        }
    }
}
