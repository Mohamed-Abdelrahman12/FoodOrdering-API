using FluentValidation;

namespace FoodOrdering.Application.Features.Carts.Commands.UpdateCartItem
{
    public class UpdateCartItemValidator : AbstractValidator<UpdateCartItemCommand>
    {
        public UpdateCartItemValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0).LessThanOrEqualTo(100);
        }
    }
}
