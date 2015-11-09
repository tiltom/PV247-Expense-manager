using System.ComponentModel.DataAnnotations;

namespace ExpenseManager.Entity.Enums
{
    /// <summary>
    ///     Type of the category
    /// </summary>
    public enum CategoryType
    {
        /// <summary>
        ///     Represents category for expenses
        /// </summary>
        [Display(Name = "Expense")] Expense,

        /// <summary>
        ///     Represents category for incomes
        /// </summary>
        [Display(Name = "Income")] Income,

        /// <summary>
        ///     Represents category for both incomes and expenses
        /// </summary>
        [Display(Name = "Income and Expense")] IncomeAndExpense
    }
}