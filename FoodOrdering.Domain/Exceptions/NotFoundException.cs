
namespace FoodOrdering.Domain.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string name, object key)
            : base($"{name} with id '{key}' was not found.") { }
    }

    public class ValidationException : Exception
    {
        public IDictionary<string, string[]> Errors { get; }

        public ValidationException(IDictionary<string, string[]> errors)
            : base("One or more validation errors occurred.")
        {
            Errors = errors;
        }
    }

    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message = "Unauthorized access.")
            : base(message) { }
    }

    public class BusinessException : Exception
    {
        public BusinessException(string message) : base(message) { }
    }
}
