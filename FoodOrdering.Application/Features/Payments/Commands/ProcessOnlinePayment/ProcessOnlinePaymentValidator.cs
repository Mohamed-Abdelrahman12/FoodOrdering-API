using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Features.Payments.Commands.ProcessOnlinePayment
{
    public class ProcessOnlinePaymentValidator : AbstractValidator<ProcessOnlinePaymentCommand>
    {
        public ProcessOnlinePaymentValidator()
        {
            RuleFor(x => x.OrderId).NotEmpty();
            RuleFor(x => x.CardNumber).NotEmpty().Length(13, 19).Matches(@"^\d+$");
            RuleFor(x => x.CardHolder).NotEmpty().MaximumLength(100);
            RuleFor(x => x.ExpiryMonth).NotEmpty().Matches(@"^(0[1-9]|1[0-2])$");
            RuleFor(x => x.ExpiryYear).NotEmpty().Matches(@"^\d{4}$");
            RuleFor(x => x.Cvv).NotEmpty().Length(3, 4).Matches(@"^\d+$");
        }
    }
}
