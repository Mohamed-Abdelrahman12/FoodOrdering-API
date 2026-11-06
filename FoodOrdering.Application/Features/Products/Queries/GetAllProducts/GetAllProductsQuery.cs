using FoodOrdering.Application.Common.Models;
using FoodOrdering.Application.Features.Products.DTOs;
using MediatR;


namespace FoodOrdering.Application.Features.Products.Queries.GetAllProducts
{
    public record GetAllProductsQuery(Guid? CategoryId = null, bool AvailableOnly = true, int Page = 1, int PageSize = 20, string? SearchTerm = null,
        int? Rating = null ,
           string? SortOrder = "asc",  
    decimal? MinPrice = null,      
    decimal? MaxPrice = null)
      : IRequest<ApiResponse<PagedResult<ReturnProductDto>>>;

}
