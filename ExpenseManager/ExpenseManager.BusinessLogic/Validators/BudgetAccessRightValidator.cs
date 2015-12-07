using ExpenseManager.Entity.Budgets;
using ExpenseManager.Resources;
using ExpenseManager.Resources.BudgetResources;
using FluentValidation;

namespace ExpenseManager.BusinessLogic.Validators
{
    internal class BudgetAccessRightValidator : AbstractValidator<BudgetAccessRight>
    {
        public BudgetAccessRightValidator()
        {
            this.RuleFor(right => right.UserProfile)
                .NotNull()
                .WithLocalizedMessage(() => SharedResource.UserNotFoundByEmail);
            this.RuleFor(right => right.Budget)
                .NotNull()
                .WithLocalizedMessage(() => BudgetAccessRightResource.BudgetNotFoundById);
        }
    }
}