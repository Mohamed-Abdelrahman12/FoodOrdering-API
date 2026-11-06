namespace FoodOrdering.API.Middleware
{
    using System.Globalization;


    public class LanguageMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly string[] SupportedLanguages = { "en", "ar" };

        public LanguageMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            var lang = context.Request.Headers["Accept-Language"].FirstOrDefault()
                ?? context.User.FindFirst("preferred_language")?.Value
                ?? "en";

            lang = SupportedLanguages.Contains(lang.ToLower()) ? lang.ToLower() : "en";

            context.Items["Language"] = lang;

            var culture = lang == "ar" ? new CultureInfo("ar-EG") : new CultureInfo("en-US");
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;

            await _next(context);
        }
    }
}
