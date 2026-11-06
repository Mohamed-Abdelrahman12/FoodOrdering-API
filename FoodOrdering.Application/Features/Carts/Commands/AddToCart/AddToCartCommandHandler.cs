using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Carts.DTOs;
using FoodOrdering.Application.Features.Carts.Mapper;
using FoodOrdering.Domain.Entities;
using FoodOrdering.Domain.Exceptions;
using FoodOrdering.Domain.Interfaces;
using FoodOrdering.Infrastructure.Repositories;
using MediatR;


namespace FoodOrdering.Application.Features.Carts.Commands.AddToCart
{
    public class AddToCarCommandtHandler : IRequestHandler<AddToCartCommand, ApiResponse<CartDto>>
    {
        private readonly IRedisCartRepository _redisCart;
        private readonly IUnitOfWork _uow;

        public AddToCarCommandtHandler(IRedisCartRepository redisCart, IUnitOfWork uow)
        {
            _redisCart = redisCart;
            _uow = uow;
        }

        public async Task<ApiResponse<CartDto>> Handle(AddToCartCommand request, CancellationToken cancellationToken)
        {
            var product = await _uow.Products.GetByIdAsync(request.ProductId, cancellationToken)
                ?? throw new NotFoundException(nameof(Product), request.ProductId);

            if (!product.IsAvailable)
                return ApiResponse<CartDto>.Fail($"'{product.NameEn}' is currently unavailable.");

            if (product.StockQuantity < request.Quantity)
                return ApiResponse<CartDto>.Fail($"Only {product.StockQuantity} units available for '{product.NameEn}'.");

            var cart = await _redisCart.GetCartAsync(request.UserId)
                       ?? new FoodOrdering.Domain.Entities.Cart { UserId = request.UserId, Items = new List<CartItem>() };

            var existing = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
            if (existing != null)
            {
                var newQty = existing.Quantity + request.Quantity;
                if (newQty > product.StockQuantity)
                    return ApiResponse<CartDto>.Fail($"Cannot add {request.Quantity} more. Only {product.StockQuantity} in stock.");
                existing.Quantity = newQty;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    Product = product
                });
            }

            await _redisCart.UpdateCartAsync(cart);

            var dto = await CartMapper.MapCartToDtoAsync(cart, _uow, cancellationToken);
            return ApiResponse<CartDto>.Ok(dto);
        }
    }
}
