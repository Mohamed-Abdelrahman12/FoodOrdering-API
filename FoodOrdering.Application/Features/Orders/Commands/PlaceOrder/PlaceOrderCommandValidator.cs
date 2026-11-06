using FluentValidation;


namespace FoodOrdering.Application.Features.Orders.Commands.PlaceOrder
{
    public class PlaceOrderCommandValidator : AbstractValidator<PlaceOrderCommand>
    {
        public PlaceOrderCommandValidator()
        {
            RuleFor(x => x.DeliveryAddress).NotEmpty().MaximumLength(500);
            RuleFor(x => x.PaymentMethod).IsInEnum();
        }
    }
}
