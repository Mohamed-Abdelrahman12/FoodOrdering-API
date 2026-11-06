using AutoMapper;
using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Payments.DTOs;
using FoodOrdering.Domain.Exceptions;
using FoodOrdering.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodOrdering.Application.Features.Payments.Queries.GetPaymentByOrder
{
    public class GetPaymentByOrderHandler : IRequestHandler<GetPaymentByOrderQuery, ApiResponse<PaymentDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetPaymentByOrderHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        public async Task<ApiResponse<PaymentDto>> Handle(GetPaymentByOrderQuery request, CancellationToken cancellationToken)
        {
            var orderQuery = _uow.Orders.Query().Where(o => o.Id == request.OrderId);
            if (!request.IsAdmin)
                orderQuery = orderQuery.Where(o => o.UserId == request.UserId);

            var order = await orderQuery.FirstOrDefaultAsync(cancellationToken)
                ?? throw new NotFoundException("Order", request.OrderId);

            var payment = await _uow.Payments.Query()
                .FirstOrDefaultAsync(p => p.OrderId == request.OrderId, cancellationToken)
                ?? throw new NotFoundException("Payment", request.OrderId);

            return ApiResponse<PaymentDto>.Ok(_mapper.Map<PaymentDto>(payment));
        }
    }
}
