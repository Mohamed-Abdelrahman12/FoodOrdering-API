using FluentValidation;

namespace FoodOrdering.Application.Features.Products.Commands.CreateProduct
{
    public sealed class CreateProductCommandValidator
        : AbstractValidator<CreateProductCommand>
    {
        private static readonly string[] AllowedTypes =
            ["image/jpeg", "image/png", "image/webp", "image/gif"];

        private const long MaxFileSizeBytes = 2 * 1024 * 1024; // 2 MB

        public CreateProductCommandValidator()
        {
            RuleFor(x => x.NameEn)
                .NotEmpty().WithMessage("English name is required.")
                .MaximumLength(200);

            RuleFor(x => x.NameAr)
                .NotEmpty().WithMessage("Arabic name is required.")
                .MaximumLength(200);

            RuleFor(x => x.DescriptionEn)
                .NotEmpty().WithMessage("English description is required.")
                .MaximumLength(2000);

            RuleFor(x => x.DescriptionAr)
                .NotEmpty().WithMessage("Arabic description is required.")
                .MaximumLength(2000);

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero.");

            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative.");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Category is required.");
 
            When(x => x.ImageUrl is not null, () =>
            {
                RuleFor(x => x.ImageUrl!.Length)
                    .LessThanOrEqualTo(MaxFileSizeBytes)
                    .WithMessage("Image must not exceed 2 MB.");

                RuleFor(x => x.ImageUrl!.ContentType)
                    .Must(ct => AllowedTypes.Contains(ct.ToLower()))
                    .WithMessage("Image must be JPEG, PNG, WEBP or GIF.");
            });
        }
    }
}