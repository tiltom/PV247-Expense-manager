using ExpenseManager.Entity.Wallets;
using FluentValidation;

namespace ExpenseManager.BusinessLogic.Validators
{
    internal class WalletAccessRightValidator : AbstractValidator<WalletAccessRight>
    {
        public WalletAccessRightValidator()
        {
            this.RuleFor(right => right.UserProfile).NotNull();
            this.RuleFor(right => right.Wallet).NotNull().SetValidator(new WalletValidator());
        }
    }
}