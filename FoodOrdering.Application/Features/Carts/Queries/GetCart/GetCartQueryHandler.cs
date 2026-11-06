using AutoMapper;
using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Carts.DTOs;
using FoodOrdering.Application.Features.Carts.Mapper;
using FoodOrdering.Domain.Interfaces;
using FoodOrdering.Infrastructure.Repositories;
using MediatR;

namespace FoodOrdering.Application.Features.Cart.Queries.GetCart
{
    public class GetCartHandler : IRequestHandler<GetCartQuery, ApiResponse<CartDto>>
    {
        private readonly IRedisCartRepository _redisCart;
        private readonly IUnitOfWork _uow;

        public GetCartHandler(IRedisCartRepository redisCart, IUnitOfWork uow)
        {
            _redisCart = redisCart;
            _uow = uow;
        }

        public async Task<ApiResponse<CartDto>> Handle(GetCartQuery request, CancellationToken cancellationToken)
        {
            var cart = await _redisCart.GetCartAsync(request.UserId);

            if (cart == null)
                return ApiResponse<CartDto>.Ok(new CartDto { UserId = request.UserId });

            var dto = await CartMapper.MapCartToDtoAsync(cart, _uow, cancellationToken);
            return ApiResponse<CartDto>.Ok(dto);
        }
    }
}
