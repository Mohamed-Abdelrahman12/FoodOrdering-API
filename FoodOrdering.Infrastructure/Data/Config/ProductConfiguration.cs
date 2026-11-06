using FoodOrdering.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodOrdering.Infrastructure.Data.Config
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.NameEn).IsRequired().HasMaxLength(200);
            builder.Property(x => x.NameAr).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Price).HasColumnType("decimal(18,2)");
            builder.HasOne(x => x.Category).WithMany(x => x.Products).HasForeignKey(x => x.CategoryId);
            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
