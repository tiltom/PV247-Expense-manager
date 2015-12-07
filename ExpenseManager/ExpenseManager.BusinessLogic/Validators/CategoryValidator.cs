using ExpenseManager.BusinessLogic.CategoryServices;
using ExpenseManager.Entity.Categories;
using FluentValidation;

namespace ExpenseManager.BusinessLogic.Validators
{
    internal class CategoryValidator : AbstractValidator<Category>
    {
        public CategoryValidator()
        {
            this.RuleFor(category => category.Name).NotNull().NotEmpty();
            this.RuleFor(category => category.Description).NotNull().NotEmpty();
            this.RuleFor(category => category.IconPath)
                .Must(iconPath => CategoryService.GetGlyphicons().Contains(iconPath));
        }
    }
}