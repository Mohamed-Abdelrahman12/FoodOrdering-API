using FoodOrdering.Application.Features.Carts.DTOs;
using FoodOrdering.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodOrdering.Application.Features.Carts.Mapper
{
    public static class CartMapper
    {
        internal static async Task<CartDto> MapCartToDtoAsync(FoodOrdering.Domain.Entities.Cart cart, IUnitOfWork uow, CancellationToken ct)
        {
            var productIds = cart.Items.Select(i => i.ProductId).Distinct().ToList();
            var products = await uow.Products.Query()
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync(ct);

            var productLookup = products.ToDictionary(p => p.Id);

            var items = cart.Items
                .Where(i => productLookup.ContainsKey(i.ProductId))
                .Select(i =>
                {
                    var p = productLookup[i.ProductId];
                    return new CartItemDto
                    {
                        ProductId = p.Id,
                        ProductNameEn = p.NameEn,
                        ProductNameAr = p.NameAr,
                        ImageUrl = p.ImageUrl,
                        Quantity = i.Quantity,
                        UnitPrice = p.Price
                    };
                }).ToList();

            return new CartDto
            {
                UserId = cart.UserId,
                Items = items
            };
        }
    }
}
