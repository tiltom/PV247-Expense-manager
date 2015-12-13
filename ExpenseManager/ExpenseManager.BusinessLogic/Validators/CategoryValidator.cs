using ExpenseManager.Entity.Categories;
using ExpenseManager.Resources.CategoryResources;
using FluentValidation;

namespace ExpenseManager.BusinessLogic.Validators
{
    internal class CategoryValidator : AbstractValidator<Category>
    {
        public CategoryValidator()
        {
            this.RuleFor(category => category.Name)
                .NotNull()
                .NotEmpty()
                .WithLocalizedMessage(() => CategoryResource.NameNotNullOrEmpty);
            this.RuleFor(category => category.Description)
                .NotNull()
                .NotEmpty()
                .WithLocalizedMessage(() => CategoryResource.DescriptionNotNullOrEmpty);
        }
    }
}