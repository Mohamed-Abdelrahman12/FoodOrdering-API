using AutoMapper;
using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Contacts.DTOs;
using FoodOrdering.Domain.Entities;
using FoodOrdering.Domain.Interfaces;
using MediatR;

namespace FoodOrdering.Application.Features.Contacts.Commands.CreateContact
{
    internal class CreateContactCommandHandler
    : IRequestHandler<CreateContactCommand, ApiResponse<ContactDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public CreateContactCommandHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<ApiResponse<ContactDto>> Handle(
            CreateContactCommand request,
            CancellationToken cancellationToken)
        {
            var contact = new Contact
            {
                FullName = request.FullName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Subject = request.Subject,
                Message = request.Message
            };

            await _uow.Contacts.AddAsync(contact, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return ApiResponse<ContactDto>.Created(
                _mapper.Map<ContactDto>(contact),
                "Your message has been received. We will get back to you soon.");
        }
    }
}
