using FoodOrdering.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodOrdering.Infrastructure.Data.Config
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.NameEn).IsRequired().HasMaxLength(100);
            builder.Property(x => x.NameAr).IsRequired().HasMaxLength(100);
            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }

}
