using FoodOrdering.Application.Common.Models;
using FoodOrdering.Domain.Entities;
using FoodOrdering.Domain.Enums;
using FoodOrdering.Domain.Exceptions;
using FoodOrdering.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace FoodOrdering.Application.Features.Orders.Commands.CancelOrder
{
    internal class CancelOrderCommandHandler  : IRequestHandler<CancelOrderCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _uow;

        public CancelOrderCommandHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<ApiResponse<bool>> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            var query = _uow.Orders.Query()
                .Include(o => o.Items).ThenInclude(i => i.Product)
                .Where(o => o.Id == request.OrderId);

            if (!request.IsAdmin)
                query = query.Where(o => o.UserId == request.UserId);

            var order = await query.FirstOrDefaultAsync(cancellationToken)
                ?? throw new NotFoundException(nameof(Order), request.OrderId);

            if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Confirmed)
                return ApiResponse<bool>.Fail("Cannot cancel an order that is already being prepared or delivered.");

            order.Status = OrderStatus.Cancelled;

            foreach (var item in order.Items)
            {
                item.Product.StockQuantity += item.Quantity;
                await _uow.Products.UpdateAsync(item.Product, cancellationToken);
            }

            await _uow.Orders.UpdateAsync(order, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.Ok(true, "Order cancelled successfully.");
        }
    }
}
