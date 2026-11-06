using FoodOrdering.Application.Features.Reviews.Commands.ApproveReview;
using FoodOrdering.Application.Features.Reviews.Commands.CreateReview;
using FoodOrdering.Application.Features.Reviews.Commands.DeleteReview;
using FoodOrdering.Application.Features.Reviews.Oueries.GetAllReviews;
using FoodOrdering.Application.Features.Reviews.Oueries.GetMyReviews;
using FoodOrdering.Application.Features.Reviews.Oueries.GetProductReviews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrdering.API.Controllers
{

    public class ReviewsController : BaseController
    {

        [HttpGet("product/{productId:guid}")]
        public async Task<IActionResult> GetProductReviews(
            Guid productId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await Mediator.Send(
                new GetProductReviewsQuery(productId, page, pageSize));
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateReviewRequest request)
        {
            var result = await Mediator.Send(new CreateReviewCommand(
                CurrentUserId,
                request.ProductId,
                request.OrderId,
                request.Rating,
                request.Comment));
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("my")]
        [Authorize]
        public async Task<IActionResult> GetMyReviews(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await Mediator.Send(
                new GetMyReviewsQuery(CurrentUserId, page, pageSize));
            return StatusCode(result.StatusCode, result);
        }


        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await Mediator.Send(
                new DeleteReviewCommand(id, CurrentUserId, IsAdmin));
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll(
            [FromQuery] bool? isApproved = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] int? rating = null,
            [FromQuery] string? productSearchTerm = null,
            [FromQuery] string? fullNameSearchTerm = null,
            [FromQuery] DateOnly? startDate = null,
            [FromQuery] DateOnly? endDate = null
            )
        {
            var result = await Mediator.Send(
                new GetAllReviewsQuery(
                    isApproved,
                    page,
                    pageSize,
                    rating,
                    productSearchTerm,
                    fullNameSearchTerm,
                    startDate,
                    endDate));
            return StatusCode(result.StatusCode, result);
        }


        [HttpPut("{id:guid}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(Guid id)
        {
            var result = await Mediator.Send(new ApproveReviewCommand(id));
            return StatusCode(result.StatusCode, result);
        }
    }

  
    public record CreateReviewRequest(
        Guid ProductId,
        Guid? OrderId,
        int Rating,
        string? Comment);
}