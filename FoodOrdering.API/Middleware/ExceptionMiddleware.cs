using FoodOrdering.Application.Common.Models;
using FoodOrdering.Domain.Exceptions;
using System.Text.Json;

namespace FoodOrdering.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = exception switch
            {
                NotFoundException e => (StatusCode: 404, Body: ApiResponse<object>.Fail(e.Message, 404)),
                ValidationException e => (StatusCode: 422, Body: ApiResponse<object>.ValidationFail(e.Errors)),
                UnauthorizedException e => (StatusCode: 401, Body: ApiResponse<object>.Fail(e.Message, 401)),
                BusinessException e => (StatusCode: 400, Body: ApiResponse<object>.Fail(e.Message, 400)),
                _ => (StatusCode: 500, Body: ApiResponse<object>.Fail("An unexpected error occurred.", 500))
            };

            context.Response.StatusCode = response.StatusCode;
            await context.Response.WriteAsync(JsonSerializer.Serialize(response.Body, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));
        }
    }
}
