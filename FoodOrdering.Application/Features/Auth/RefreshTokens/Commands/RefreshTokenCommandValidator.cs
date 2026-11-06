using FluentValidation;

namespace FoodOrdering.Application.Features.Auth.RefreshTokens.Commands
{
    public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenCommandValidator()
        {
            RuleFor(x => x.RefreshToken)
                           .NotEmpty()
                           .WithMessage("Refresh token is required.");
        }
    }
}
