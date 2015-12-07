using ExpenseManager.Entity.Budgets;
using FluentValidation;

namespace ExpenseManager.BusinessLogic.Validators
{
    internal class BudgetValidator : AbstractValidator<Budget>
    {
        public BudgetValidator()
        {
            this.RuleFor(budget => budget.Name).NotNull().NotEmpty();
            this.RuleFor(budget => budget.StartDate).LessThanOrEqualTo(budget => budget.EndDate);
            this.RuleFor(budget => budget.Limit).GreaterThan(0);
        }
    }
}