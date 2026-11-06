using FoodOrdering.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodOrdering.Infrastructure.Data.Config
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Rating)
                .IsRequired();
            builder.Property(x => x.OrderId);
            builder.Property(x => x.Comment)
                .HasMaxLength(1000);

            // One user can review one product only once
            builder.HasIndex(x => new { x.UserId, x.ProductId })
                .IsUnique()
                .HasDatabaseName("IX_Reviews_User_Product_Unique");

            builder.HasOne(x => x.User)
               .WithMany(u => u.Reviews)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Product)
                  .WithMany(u => u.Reviews)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            //builder.HasOne(x => x.Order)
            //      .WithMany(u => u.Reviews)
            //    .HasForeignKey(x => x.OrderId)
            //    .OnDelete(DeleteBehavior.NoAction)
            //    .IsRequired(false);

            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}