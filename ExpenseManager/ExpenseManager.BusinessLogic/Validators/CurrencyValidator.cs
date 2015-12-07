using ExpenseManager.Entity.Currencies;
using FluentValidation;

namespace ExpenseManager.BusinessLogic.Validators
{
    internal class CurrencyValidator : AbstractValidator<Currency>
    {
        public CurrencyValidator()
        {
            this.RuleFor(currency => currency.Code).NotNull().NotEmpty();
            this.RuleFor(currency => currency.Name).NotNull().NotEmpty();
            this.RuleFor(currency => currency.Symbol).NotNull().NotEmpty();
        }
    }
}