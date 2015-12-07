using ExpenseManager.Entity.Wallets;
using ExpenseManager.Resources;
using FluentValidation;

namespace ExpenseManager.BusinessLogic.Validators
{
    internal class WalletAccessRightValidator : AbstractValidator<WalletAccessRight>
    {
        public WalletAccessRightValidator()
        {
            this.RuleFor(right => right.UserProfile)
                .NotNull()
                .WithLocalizedMessage(() => SharedResource.UserNotFoundByEmail);
            this.RuleFor(right => right.Wallet)
                .NotNull()
                .SetValidator(new WalletValidator());
        }
    }
}