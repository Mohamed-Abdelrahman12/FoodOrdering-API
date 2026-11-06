using AutoMapper;
using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Orders.DTOs;
using FoodOrdering.Domain.Entities;
using FoodOrdering.Domain.Exceptions;
using FoodOrdering.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace FoodOrdering.Application.Features.Orders.Queries.GetOrderById
{
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, ApiResponse<OrderDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetOrderByIdQueryHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        public async Task<ApiResponse<OrderDto>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Orders.Query()
                .Include(o => o.Items).ThenInclude(i => i.Product)
                .Include(o => o.Payment)
                .Include(o => o.User)
                .Where(o => o.Id == request.OrderId);

            if (!request.IsAdmin)
                query = query.Where(o => o.UserId == request.UserId);

            var order = await query.FirstOrDefaultAsync(cancellationToken)
                ?? throw new NotFoundException(nameof(Order), request.OrderId);

            return ApiResponse<OrderDto>.Ok(_mapper.Map<OrderDto>(order));
        }
    }
}
