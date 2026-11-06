using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Carts.DTOs;
using FoodOrdering.Application.Features.Carts.Mapper;
using FoodOrdering.Domain.Exceptions;
using FoodOrdering.Domain.Interfaces;
using FoodOrdering.Infrastructure.Repositories;
using MediatR;

namespace FoodOrdering.Application.Features.Carts.Commands.RemoveCartItem
{
    public class RemoveCartItemHandler : IRequestHandler<RemoveCartItemCommand, ApiResponse<CartDto>>
    {
        private readonly IRedisCartRepository _redisCart;
        private readonly IUnitOfWork _uow;

        public RemoveCartItemHandler(IRedisCartRepository redisCart, IUnitOfWork uow)
        {
            _redisCart = redisCart;
            _uow = uow;
        }

        public async Task<ApiResponse<CartDto>> Handle(RemoveCartItemCommand request, CancellationToken cancellationToken)
        {
            var cart = await _redisCart.GetCartAsync(request.UserId)
                ?? throw new NotFoundException("Cart", request.UserId);

            var item = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
            if (item != null)
            {
                cart.Items.Remove(item);
                await _redisCart.UpdateCartAsync(cart);
            }

            var dto = await CartMapper.MapCartToDtoAsync(cart, _uow, cancellationToken);
            return ApiResponse<CartDto>.Ok(dto);
        }
    }
}
