using AutoMapper;
using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Carts.DTOs;
using FoodOrdering.Application.Features.Carts.Mapper;
using FoodOrdering.Domain.Exceptions;
using FoodOrdering.Domain.Interfaces;
using FoodOrdering.Infrastructure.Repositories;
using MediatR;


namespace FoodOrdering.Application.Features.Carts.Commands.UpdateCartItem
{
    public class UpdateCartItemHandler : IRequestHandler<UpdateCartItemCommand, ApiResponse<CartDto>>
    {
        private readonly IRedisCartRepository _redisCart;
        private readonly IUnitOfWork _uow;

        public UpdateCartItemHandler(IRedisCartRepository redisCart, IUnitOfWork uow)
        {
            _redisCart = redisCart;
            _uow = uow;
        }

        public async Task<ApiResponse<CartDto>> Handle(UpdateCartItemCommand request, CancellationToken cancellationToken)
        {
            var cart = await _redisCart.GetCartAsync(request.UserId)
                ?? throw new NotFoundException("Cart", request.UserId);

            var item = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId)
                ?? throw new NotFoundException("CartItem", request.ProductId);

            if (request.Quantity == 0)
                cart.Items.Remove(item);
            else
            {
                var product = await _uow.Products.GetByIdAsync(request.ProductId, cancellationToken);
                if (product != null && request.Quantity > product.StockQuantity)
                    return ApiResponse<CartDto>.Fail($"Only {product.StockQuantity} units available.");

                item.Quantity = request.Quantity;
            }

            await _redisCart.UpdateCartAsync(cart);

            var dto = await CartMapper.MapCartToDtoAsync(cart, _uow, cancellationToken);
            return ApiResponse<CartDto>.Ok(dto);
        }
    }

}
