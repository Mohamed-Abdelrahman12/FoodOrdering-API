using FoodOrdering.Application.Common.Models;
using FoodOrdering.Domain.Entities;
using FoodOrdering.Domain.Exceptions;
using FoodOrdering.Domain.Interfaces;
using MediatR;

namespace FoodOrdering.Application.Features.Products.Commands.DeleteProduct
{
    internal class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _uow;

        public DeleteProductCommandHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<ApiResponse<bool>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _uow.Products.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Product), request.Id);

            product.IsDeleted = true;
            await _uow.Products.UpdateAsync(product, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.Ok(true, "Product deleted.");
        }
    }
}
