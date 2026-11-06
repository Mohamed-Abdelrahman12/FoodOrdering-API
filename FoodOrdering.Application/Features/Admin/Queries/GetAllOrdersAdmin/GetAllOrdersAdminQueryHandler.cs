using AutoMapper;
using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Orders.DTOs;
using FoodOrdering.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace FoodOrdering.Application.Features.Admin.Queries.GetAllOrdersAdmin
{
    internal class GetAllOrdersAdminQueryHandler : IRequestHandler<GetAllOrdersAdminQuery, ApiResponse<PagedResult<OrderDto>>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetAllOrdersAdminQueryHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        public async Task<ApiResponse<PagedResult<OrderDto>>> Handle(GetAllOrdersAdminQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Orders.Query()
                .Include(o => o.Items).ThenInclude(i => i.Product)
                .Include(o => o.Payment)
                .Include(o => o.User)
                .OrderByDescending(o => o.CreatedAt)
                .AsQueryable();

            if (request.StatusFilter.HasValue)
                query = query.Where(o => o.Status == request.StatusFilter.Value);

            var totalCount = await query.CountAsync(cancellationToken);
            var orders = await query.Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize).ToListAsync(cancellationToken);

            return ApiResponse<PagedResult<OrderDto>>.Ok(new PagedResult<OrderDto>
            {
                Items = _mapper.Map<IEnumerable<OrderDto>>(orders),
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            });
        }
    }
}
