using AutoMapper;
using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Contacts.DTOs;
using FoodOrdering.Domain.Entities;
using FoodOrdering.Domain.Exceptions;
using FoodOrdering.Domain.Interfaces;
using MediatR;


namespace FoodOrdering.Application.Features.Contacts.Queries.GetContactById
{
    public class GetContactByIdQueryHandler
      : IRequestHandler<GetContactByIdQuery, ApiResponse<ContactDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetContactByIdQueryHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<ApiResponse<ContactDto>> Handle(
            GetContactByIdQuery request,
            CancellationToken cancellationToken)
        {
            var contact = await _uow.Contacts
                .GetByIdAsync(request.ContactId, cancellationToken)
                ?? throw new NotFoundException(nameof(Contact), request.ContactId);

            // Auto-mark as read the first time an admin opens it
            if (!contact.IsRead)
            {
                contact.IsRead = true;
                contact.UpdatedAt = DateTime.UtcNow;

                await _uow.Contacts.UpdateAsync(contact, cancellationToken);
                await _uow.SaveChangesAsync(cancellationToken);
            }

            return ApiResponse<ContactDto>.Ok(_mapper.Map<ContactDto>(contact));
        }
    }
}
