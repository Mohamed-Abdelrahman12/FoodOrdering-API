

namespace FoodOrdering.Application.Common.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public IDictionary<string, string[]>? Errors { get; set; }
        public int StatusCode { get; set; }

        public static ApiResponse<T> Ok(T data, string message = "Success")
            => new() { Success = true, Data = data, Message = message, StatusCode = 200 };

        public static ApiResponse<T> Created(T data, string message = "Created successfully")
            => new() { Success = true, Data = data, Message = message, StatusCode = 201 };

        public static ApiResponse<T> Fail(string message, int statusCode = 400)
            => new() { Success = false, Message = message, StatusCode = statusCode };

        public static ApiResponse<T> ValidationFail(IDictionary<string, string[]> errors)
            => new() { Success = false, Message = "Validation failed", Errors = errors, StatusCode = 422 };
    }
}
