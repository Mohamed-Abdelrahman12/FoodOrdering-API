using FluentValidation;
namespace FoodOrdering.Application.Features.Categories.Commands.UpdateCategory
{
    public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
    {
        public UpdateCategoryCommandValidator()
        {
            RuleFor(x => x.NameEn).NotEmpty().MaximumLength(100);
            RuleFor(x => x.NameAr).NotEmpty().MaximumLength(100);
        }
    }
}
