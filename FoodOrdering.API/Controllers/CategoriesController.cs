using FoodOrdering.Application.Features.Categories.Commands.CreateCategory;
using FoodOrdering.Application.Features.Categories.Commands.DeleteCategory;
using FoodOrdering.Application.Features.Categories.Commands.UpdateCategory;
using FoodOrdering.Application.Features.Categories.Queries.GetAllCategories;
using FoodOrdering.Application.Features.Categories.Queries.GetCategoryById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrdering.API.Controllers
{
    public class CategoriesController : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool activeOnly = true)
        {
            var result = await Mediator.Send(new GetAllCategoriesQuery(activeOnly));
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await Mediator.Send(new GetCategoryByIdQuery(id));
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromForm] CreateCategoryCommand command)
        {
            var result = await Mediator.Send(command);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromForm] UpdateCategoryCommand command)
        {
            var result = await Mediator.Send(command with { Id = id });
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await Mediator.Send(new DeleteCategoryCommand(id));
            return StatusCode(result.StatusCode, result);
        }
    }
}
