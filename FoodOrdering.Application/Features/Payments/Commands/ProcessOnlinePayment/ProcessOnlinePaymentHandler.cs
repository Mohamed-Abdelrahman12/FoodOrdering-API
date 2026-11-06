using AutoMapper;
using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Payments.DTOs;
using FoodOrdering.Domain.Enums;
using FoodOrdering.Domain.Exceptions;
using FoodOrdering.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Features.Payments.Commands.ProcessOnlinePayment
{
    public class ProcessOnlinePaymentHandler : IRequestHandler<ProcessOnlinePaymentCommand, ApiResponse<PaymentDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public ProcessOnlinePaymentHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

        public async Task<ApiResponse<PaymentDto>> Handle(ProcessOnlinePaymentCommand request, CancellationToken cancellationToken)
        {
            var order = await _uow.Orders.Query()
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.Id == request.OrderId && o.UserId == request.UserId, cancellationToken)
                ?? throw new NotFoundException("Order", request.OrderId);

            if (order.Payment == null)
                return ApiResponse<PaymentDto>.Fail("Payment record not found for this order.");

            if (order.Payment.Status == PaymentStatus.Paid)
                return ApiResponse<PaymentDto>.Fail("Order is already paid.");

            if (order.Payment.Method != PaymentMethod.Online)
                return ApiResponse<PaymentDto>.Fail("This order is set for Cash on Delivery.");

            var isPaymentSuccessful = SimulatePaymentGateway(request.CardNumber, request.Cvv);

            if (!isPaymentSuccessful)
            {
                order.Payment.Status = PaymentStatus.Failed;
                await _uow.Payments.UpdateAsync(order.Payment, cancellationToken);
                await _uow.SaveChangesAsync(cancellationToken);
                return ApiResponse<PaymentDto>.Fail("Payment failed. Please check your card details.");
            }

            order.Payment.Status = PaymentStatus.Paid;
            order.Payment.PaidAt = DateTime.UtcNow;
            order.Payment.TransactionId = $"TXN-{Guid.NewGuid().ToString()[..8].ToUpper()}";

            await _uow.Payments.UpdateAsync(order.Payment, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return ApiResponse<PaymentDto>.Ok(_mapper.Map<PaymentDto>(order.Payment), "Payment processed successfully.");
        }

        private static bool SimulatePaymentGateway(string cardNumber, string cvv)
            => !cardNumber.StartsWith("0000");
    }
}
