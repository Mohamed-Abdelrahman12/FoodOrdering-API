using AutoMapper;
using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Contacts.DTOs;
using FoodOrdering.Domain.Entities;
using FoodOrdering.Domain.Exceptions;
using FoodOrdering.Domain.Interfaces;
using MediatR;


namespace FoodOrdering.Application.Features.Contacts.Commands.MarkAsRead
{
    internal class MarkAsReadCommandHandler : IRequestHandler<MarkAsReadCommand, ApiResponse<ContactDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public MarkAsReadCommandHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<ApiResponse<ContactDto>> Handle(
            MarkAsReadCommand request,
            CancellationToken cancellationToken)
        {
            var contact = await _uow.Contacts.GetByIdAsync(request.ContactId, cancellationToken)
                ?? throw new NotFoundException(nameof(Contact), request.ContactId);

            contact.IsRead = true;

            if (request.IsReplied)
            {
                contact.IsReplied = true;
                contact.RepliedAt = DateTime.UtcNow;
            }

            if (!string.IsNullOrWhiteSpace(request.AdminNotes))
                contact.AdminNotes = request.AdminNotes;

            await _uow.Contacts.UpdateAsync(contact, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return ApiResponse<ContactDto>.Ok(
                _mapper.Map<ContactDto>(contact),
                "Contact message updated.");
        }
    }
}
