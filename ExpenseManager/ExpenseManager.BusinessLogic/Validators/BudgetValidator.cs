using ExpenseManager.Entity.Budgets;
using ExpenseManager.Resources.BudgetResources;
using FluentValidation;

namespace ExpenseManager.BusinessLogic.Validators
{
    internal class BudgetValidator : AbstractValidator<Budget>
    {
        public BudgetValidator()
        {
            this.RuleFor(budget => budget.Name)
                .NotNull()
                .NotEmpty()
                .WithLocalizedMessage(() => BudgetResource.NameNotNull);
            this.RuleFor(budget => budget.StartDate)
                .LessThanOrEqualTo(budget => budget.EndDate)
                .WithLocalizedMessage(() => BudgetResource.WrongDateSpan);
            this.RuleFor(budget => budget.Limit)
                .GreaterThan(0)
                .WithLocalizedMessage(() => BudgetResource.LimitGreaterThanZero);
        }
    }
}