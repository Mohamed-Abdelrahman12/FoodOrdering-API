//using AutoMapper;
//using FoodOrdering.Application.Common.Models;
//using FoodOrdering.Application.Features.Orders.DTOs;
//using FoodOrdering.Domain.Entities;
//using FoodOrdering.Domain.Enums;
//using FoodOrdering.Domain.Interfaces;
//using MediatR;
//using Microsoft.EntityFrameworkCore;

//namespace FoodOrdering.Application.Features.Orders.Commands.PlaceOrder
//{
//    internal class PlaceOrderCommandHandler : IRequestHandler<PlaceOrderCommand, ApiResponse<OrderDto>>
//    {
//        private readonly IUnitOfWork _uow;
//        private readonly IMapper _mapper;

//        public PlaceOrderCommandHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

//        public async Task<ApiResponse<OrderDto>> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
//        {
//            var cart = await _uow.Carts.Query()
//                .Include(c => c.Items).ThenInclude(i => i.Product)
//                .FirstOrDefaultAsync(c => c.UserId == request.UserId, cancellationToken);

//            if (cart == null || !cart.Items.Any())
//                return ApiResponse<OrderDto>.Fail("Cart is empty.");

//            foreach (var item in cart.Items)
//            {
//                if (!item.Product.IsAvailable || item.Product.StockQuantity < item.Quantity)
//                    return ApiResponse<OrderDto>.Fail($"Product '{item.Product.NameEn}' is not available in requested quantity.");
//            }

//            await _uow.BeginTransactionAsync(cancellationToken);
//            try
//            {
//                var order = new Order
//                {
//                    UserId = request.UserId,
//                    DeliveryAddress = request.DeliveryAddress,
//                    Notes = request.Notes,
//                    Status = OrderStatus.Pending,
//                    TotalAmount = cart.TotalAmount
//                };

//                await _uow.Orders.AddAsync(order, cancellationToken);
//                await _uow.SaveChangesAsync(cancellationToken);

//                foreach (var cartItem in cart.Items)
//                {
//                    var orderItem = new OrderItem
//                    {
//                        OrderId = order.Id,
//                        ProductId = cartItem.ProductId,
//                        Quantity = cartItem.Quantity,
//                        UnitPrice = cartItem.Product.Price
//                    };
//                    await _uow.OrderItems.AddAsync(orderItem, cancellationToken);

//                    cartItem.Product.StockQuantity -= cartItem.Quantity;
//                    await _uow.Products.UpdateAsync(cartItem.Product, cancellationToken);
//                }

//                var payment = new Payment
//                {
//                    OrderId = order.Id,
//                    Amount = order.TotalAmount,
//                    Method = request.PaymentMethod,
//                    Status = PaymentStatus.Pending
//                };

//                await _uow.Payments.AddAsync(payment, cancellationToken);

//                foreach (var item in cart.Items.ToList())
//                    await _uow.CartItems.DeleteAsync(item, cancellationToken);

//                await _uow.SaveChangesAsync(cancellationToken);
//                await _uow.CommitTransactionAsync(cancellationToken);

//                var createdOrder = await _uow.Orders.Query()
//                    .Include(o => o.Items).ThenInclude(i => i.Product)
//                    .Include(o => o.Payment)
//                    .Include(o => o.User)
//                    .FirstOrDefaultAsync(o => o.Id == order.Id, cancellationToken);

//                return ApiResponse<OrderDto>.Created(_mapper.Map<OrderDto>(createdOrder));
//            }
//            catch
//            {
//                await _uow.RollbackTransactionAsync(cancellationToken);
//                throw;
//            }
//        }
//    }
//}
using AutoMapper;
using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Orders.DTOs;
using FoodOrdering.Domain.Entities;
using FoodOrdering.Domain.Enums;
using FoodOrdering.Domain.Interfaces;
using FoodOrdering.Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodOrdering.Application.Features.Orders.Commands.PlaceOrder
{
    internal class PlaceOrderCommandHandler : IRequestHandler<PlaceOrderCommand, ApiResponse<OrderDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IRedisCartRepository _redisCart;

        public PlaceOrderCommandHandler(IUnitOfWork uow, IMapper mapper, IRedisCartRepository redisCart)
        {
            _uow = uow;
            _mapper = mapper;
            _redisCart = redisCart;
        }

        public async Task<ApiResponse<OrderDto>> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
        {
            // 1. Load cart from Redis (source of truth for cart data)
            var cart = await _redisCart.GetCartAsync(request.UserId);

            if (cart == null || !cart.Items.Any())
                return ApiResponse<OrderDto>.Fail("Cart is empty.");

            // 2. Re-validate each cart item against the live DB products
            //    (prices/stock may have changed since the item was added)
            var productIds = cart.Items.Select(i => i.ProductId).ToList();
            var freshProducts = await _uow.Products
                .Query()
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, cancellationToken);

            foreach (var item in cart.Items)
            {
                if (!freshProducts.TryGetValue(item.ProductId, out var product))
                    return ApiResponse<OrderDto>.Fail($"Product '{item.ProductId}' no longer exists.");

                if (!product.IsAvailable)
                    return ApiResponse<OrderDto>.Fail($"'{product.NameEn}' is currently unavailable.");

                if (product.StockQuantity < item.Quantity)
                    return ApiResponse<OrderDto>.Fail(
                        $"Only {product.StockQuantity} unit(s) available for '{product.NameEn}'.");

                // Attach fresh product so unit prices are always current
                item.Product = product;
            }

            // 3. Compute total from fresh product prices (never trust cached price)
            var totalAmount = cart.Items.Sum(i => i.Product.Price * i.Quantity);

            await _uow.BeginTransactionAsync(cancellationToken);
            try
            {
                // 4. Persist the order
                var order = new Order
                {
                    UserId = request.UserId,
                    DeliveryAddress = request.DeliveryAddress,
                    Notes = request.Notes,
                    Status = OrderStatus.Pending,
                    TotalAmount = totalAmount
                };

                await _uow.Orders.AddAsync(order, cancellationToken);
                await _uow.SaveChangesAsync(cancellationToken);

                // 5. Create order items and decrement stock
                foreach (var cartItem in cart.Items)
                {
                    await _uow.OrderItems.AddAsync(new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        UnitPrice = cartItem.Product.Price
                    }, cancellationToken);

                    cartItem.Product.StockQuantity -= cartItem.Quantity;
                    await _uow.Products.UpdateAsync(cartItem.Product, cancellationToken);
                }

                // 6. Create payment record
                await _uow.Payments.AddAsync(new Payment
                {
                    OrderId = order.Id,
                    Amount = order.TotalAmount,
                    Method = request.PaymentMethod,
                    Status = PaymentStatus.Pending
                }, cancellationToken);

                await _uow.SaveChangesAsync(cancellationToken);
                await _uow.CommitTransactionAsync(cancellationToken);

                // 7. Clear the Redis cart only after DB transaction succeeds
                await _redisCart.DeleteCartAsync(request.UserId);

                // 8. Return the fully-loaded order DTO
                var createdOrder = await _uow.Orders.Query()
                    .Include(o => o.Items).ThenInclude(i => i.Product)
                    .Include(o => o.Payment)
                    .Include(o => o.User)
                    .FirstOrDefaultAsync(o => o.Id == order.Id, cancellationToken);

                return ApiResponse<OrderDto>.Created(_mapper.Map<OrderDto>(createdOrder));
            }
            catch
            {
                await _uow.RollbackTransactionAsync(cancellationToken);
                // Redis cart is intentionally NOT deleted here so the user
                // can retry without losing their cart
                throw;
            }
        }
    }
}