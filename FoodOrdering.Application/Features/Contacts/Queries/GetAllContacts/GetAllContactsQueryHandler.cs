
using AutoMapper;
using FoodOrdering.Application.Common.Filtering;
using FoodOrdering.Application.Common.Filtering.Rules;
using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Contacts.DTOs;
using FoodOrdering.Domain.Entities;
using FoodOrdering.Domain.Interfaces;
using MediatR;
namespace FoodOrdering.Application.Features.Contacts.Queries.GetAllContacts
{
    public class GetAllContactsQueryHandler
        : IRequestHandler<GetAllContactsQuery, ApiResponse<PagedResult<ContactDto>>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetAllContactsQueryHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<ApiResponse<PagedResult<ContactDto>>> Handle(
            GetAllContactsQuery request,
            CancellationToken cancellationToken)
        {
            var query = _uow.Contacts.Query().AsQueryable();

            IFilterRule<Contact>[] rules =
            {
                new BooleanFilterRule<Contact>(
                    c => c.IsRead,
                    request.IsRead),

                new BooleanFilterRule<Contact>(
                    c => c.IsReplied,
                    request.IsReplied),

                new DateRangeFilterRule<Contact>(
                    c => c.CreatedAt,
                    request.StartDate,
                    request.EndDate),

                new KeywordSearchFilterRule<Contact>(
                    request.MessageContent,
                    word => c => c.Subject.ToLower().Contains(word) ||
                                 c.Message.ToLower().Contains(word)),

                new KeywordSearchFilterRule<Contact>(
                    request.FullNameSearchTerm,
                    word => c => c.FullName.ToLower().Contains(word)),

                new KeywordSearchFilterRule<Contact>(
                    request.Email,
                    word => c => c.Email.ToLower().Contains(word)),
            };

            foreach (var rule in rules.Where(r => r.IsApplicable()))
                query = rule.Apply(query);

            var paged = await PaginationHelper.ToPagedResultAsync(
                query,
                request.Page,
                request.PageSize,
                entities => _mapper.Map<IEnumerable<ContactDto>>(entities),
                q => q.OrderByDescending(c => c.CreatedAt),
                cancellationToken);

            return ApiResponse<PagedResult<ContactDto>>.Ok(paged);
        }
    }
}