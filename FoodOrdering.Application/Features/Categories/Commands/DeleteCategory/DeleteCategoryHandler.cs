using FoodOrdering.Application.Common.Models;
using FoodOrdering.Domain.Entities;
using FoodOrdering.Domain.Exceptions;
using FoodOrdering.Domain.Interfaces;
using FoodOrdering.Infrastructure.Caching;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace FoodOrdering.Application.Features.Categories.Commands.DeleteCategory
{
    public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _uow;
        private readonly ICacheService _cache;

        public DeleteCategoryHandler(IUnitOfWork uow, ICacheService cache) { _uow = uow; _cache = cache; }

        public async Task<ApiResponse<bool>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _uow.Categories.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Category), request.Id);
            await _uow.BeginTransactionAsync(cancellationToken);

            try
            {
                // 1. Get all products in this category
                var products = await _uow.Products
                    .Query()
                    .Where(p => p.CategoryId == request.Id && !p.IsDeleted)
                    .ToListAsync(cancellationToken);

                // 2. Soft delete all products in this category
                foreach (var product in products)
                {
                    product.IsDeleted = true;
                  
                    await _uow.Products.UpdateAsync(product, cancellationToken);
                }

                // 3. Soft delete the category
                category.IsDeleted = true;
                await _uow.Categories.UpdateAsync(category, cancellationToken);

                // 4. Save all changes
                await _uow.SaveChangesAsync(cancellationToken);
                await _uow.CommitTransactionAsync(cancellationToken);

                // 5. Clear cache
                _cache.Remove("categories_true");
                _cache.Remove("categories_false");
                return ApiResponse<bool>.Ok(true,
                    $"Category '{category.NameEn}' and {products.Count} product(s) soft deleted successfully.");
            }
            catch
            {
                await _uow.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
    

