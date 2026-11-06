using AutoMapper;
using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Orders.DTOs;
using FoodOrdering.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Features.Orders.Queries.GetUserOrders
{
    public class GetUserOrdersHandler : IRequestHandler<GetUserOrdersQuery, ApiResponse<PagedResult<OrderDto>>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetUserOrdersHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        public async Task<ApiResponse<PagedResult<OrderDto>>> Handle(GetUserOrdersQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Orders.Query()
                .Include(o => o.Items).ThenInclude(i => i.Product)
                .Include(o => o.Payment)
                .Include(o => o.User)
                .Where(o => o.UserId == request.UserId)
                .OrderByDescending(o => o.CreatedAt);

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
