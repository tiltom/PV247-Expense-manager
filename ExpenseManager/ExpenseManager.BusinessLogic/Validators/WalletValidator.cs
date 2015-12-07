using ExpenseManager.Entity.Wallets;
using ExpenseManager.Resources.WalletResources;
using FluentValidation;

namespace ExpenseManager.BusinessLogic.Validators
{
    internal class WalletValidator : AbstractValidator<Wallet>
    {
        public WalletValidator()
        {
            this.RuleFor(wallet => wallet.Currency)
                .NotNull()
                .SetValidator(new CurrencyValidator());
            this.RuleFor(wallet => wallet.Name)
                .NotNull()
                .NotEmpty()
                .WithLocalizedMessage(() => WalletResource.NameNotNullOrEmpty);
        }
    }
}