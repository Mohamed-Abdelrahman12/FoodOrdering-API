using AutoMapper;
using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Orders.DTOs;
using FoodOrdering.Domain.Entities;
using FoodOrdering.Domain.Enums;
using FoodOrdering.Domain.Exceptions;
using FoodOrdering.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace FoodOrdering.Application.Features.Orders.Commands.UpdateOrderStatus
{
    public class UpdateOrderStatusHandler : IRequestHandler<UpdateOrderStatusCommand, ApiResponse<OrderDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public UpdateOrderStatusHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        public async Task<ApiResponse<OrderDto>> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var order = await _uow.Orders.Query()
                .Include(o => o.Items).ThenInclude(i => i.Product)
                .Include(o => o.Payment)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken)
                ?? throw new NotFoundException(nameof(Order), request.OrderId);

            order.Status = request.NewStatus;

            if (request.NewStatus == OrderStatus.Delivered && order.Payment?.Method == PaymentMethod.CashOnDelivery)
            {
                if (order.Payment != null)
                {
                    order.Payment.Status = PaymentStatus.Paid;
                    order.Payment.PaidAt = DateTime.UtcNow;
                    await _uow.Payments.UpdateAsync(order.Payment, cancellationToken);
                }
            }

            await _uow.Orders.UpdateAsync(order, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return ApiResponse<OrderDto>.Ok(_mapper.Map<OrderDto>(order));
        }
    }
}
