using ExpenseManager.Entity.Budgets;
using FluentValidation;

namespace ExpenseManager.BusinessLogic.Validators
{
    internal class BudgetAccessRightValidator : AbstractValidator<BudgetAccessRight>
    {
        public BudgetAccessRightValidator()
        {
            this.RuleFor(right => right.UserProfile).NotNull();
            this.RuleFor(right => right.Budget).NotNull();
        }
    }
}