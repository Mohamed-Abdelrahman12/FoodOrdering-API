//using FoodOrdering.Application.Features.Products.Commands.CreateProduct;
//using FoodOrdering.Application.Features.Products.Commands.DeleteProduct;
//using FoodOrdering.Application.Features.Products.Commands.UpdateProduct;
//using FoodOrdering.Application.Features.Products.Queries.GetAllProducts;
//using FoodOrdering.Application.Features.Products.Queries.GetProductById;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;

//namespace FoodOrdering.API.Controllers
//{
//    public class ProductsController : BaseController
//    {
//        [HttpGet]
//        public async Task<IActionResult> GetAll(
//            [FromQuery] Guid? categoryId,
//            [FromQuery] bool availableOnly = true,
//            [FromQuery] int page = 1,
//            [FromQuery] int pageSize = 20)
//        {
//            var result = await Mediator.Send(new GetAllProductsQuery(categoryId, availableOnly, page, pageSize));
//            return StatusCode(result.StatusCode, result);
//        }

//        [HttpGet("{id:guid}")]
//        public async Task<IActionResult> GetById(Guid id)
//        {
//            var result = await Mediator.Send(new GetProductByIdQuery(id));
//            return StatusCode(result.StatusCode, result);
//        }

//        [HttpPost]
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> Create([FromForm] CreateProductCommand command)
//        {
//            var result = await Mediator.Send(command);
//            return StatusCode(result.StatusCode, result);
//        }

//        [HttpPut("{id:guid}")]
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> Update(Guid id, [FromForm] UpdateProductCommand command)
//        {
//            var result = await Mediator.Send(command with { Id = id });
//            return StatusCode(result.StatusCode, result);
//        }

//        [HttpDelete("{id:guid}")]
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> Delete(Guid id)
//        {
//            var result = await Mediator.Send(new DeleteProductCommand(id));
//            return StatusCode(result.StatusCode, result);
//        }
//    }
//}
using FoodOrdering.Application.Features.Products.Commands.CreateProduct;
using FoodOrdering.Application.Features.Products.Commands.DeleteProduct;
using FoodOrdering.Application.Features.Products.Commands.UpdateProduct;
using FoodOrdering.Application.Features.Products.Queries.GetAllProducts;
using FoodOrdering.Application.Features.Products.Queries.GetProductById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrdering.API.Controllers
{
    public class ProductsController : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] Guid? categoryId,
            [FromQuery] bool availableOnly = true,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? searchTerm = null,
            [FromQuery] int? Rating = null,
            [FromQuery] string? sortOrder = "asc",    
            [FromQuery] decimal? minPrice = null,      
            [FromQuery] decimal? maxPrice = null)    
            
        {
            var result = await Mediator.Send(
                new GetAllProductsQuery(categoryId, availableOnly, page, pageSize, searchTerm, Rating, sortOrder,minPrice,maxPrice));
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await Mediator.Send(new GetProductByIdQuery(id));
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Consumes("multipart/form-data")]   
        public async Task<IActionResult> Create([FromForm] CreateProductCommand command)
        {
            var result = await Mediator.Send(command);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        [Consumes("multipart/form-data")]   
        public async Task<IActionResult> Update(Guid id, [FromForm] UpdateProductCommand command)
        {
            command.Id = id;
            var result = await Mediator.Send(command);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await Mediator.Send(new DeleteProductCommand(id));
            return StatusCode(result.StatusCode, result);
        }
    }
}