namespace FoodOrdering.API.Middleware
{

    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var start = DateTime.UtcNow;
            _logger.LogInformation("[{Time}] {Method} {Path} started", start, context.Request.Method, context.Request.Path);

            await _next(context);

            var elapsed = (DateTime.UtcNow - start).TotalMilliseconds;
            _logger.LogInformation("[{Time}] {Method} {Path} completed {StatusCode} in {Elapsed}ms",
                DateTime.UtcNow, context.Request.Method, context.Request.Path, context.Response.StatusCode, elapsed);
        }
    }
}
