using FoodOrdering.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodOrdering.Infrastructure.Data.Config
{
    public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Cart).WithMany(x => x.Items).HasForeignKey(x => x.CartId);
            builder.HasOne(x => x.Product).WithMany(x => x.CartItems).HasForeignKey(x => x.ProductId);
            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
