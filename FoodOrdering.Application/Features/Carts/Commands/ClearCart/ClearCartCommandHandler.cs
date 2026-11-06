using FoodOrdering.Application.Common.Models;
using FoodOrdering.Domain.Interfaces;
using FoodOrdering.Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodOrdering.Application.Features.Carts.Commands.ClearCart
{
    public class ClearCartHandler : IRequestHandler<ClearCartCommand, ApiResponse<bool>>
    {
        private readonly IRedisCartRepository _redisCart;

        public ClearCartHandler(IRedisCartRepository redisCart) => _redisCart = redisCart;

        public async Task<ApiResponse<bool>> Handle(ClearCartCommand request, CancellationToken cancellationToken)
        {
            await _redisCart.DeleteCartAsync(request.UserId);
            return ApiResponse<bool>.Ok(true, "Cart cleared.");
        }
    }

}
