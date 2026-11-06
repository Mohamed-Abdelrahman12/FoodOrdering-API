using FluentValidation;
namespace FoodOrdering.Application.Features.Categories.Commands.CreateCategory
{
    public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryCommandValidator()
        {
            RuleFor(x => x.NameEn).NotEmpty().MaximumLength(100);
            RuleFor(x => x.NameAr).NotEmpty().MaximumLength(100);
        }
    }
}
