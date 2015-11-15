using System;

namespace ExpenseManager.BusinessLogic
{
    public class BudgetService
    {
        /// <summary>
        ///     Additional validation for model
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static bool ValidateModel(DateTime startDate, DateTime endDate)
        {
            return startDate <= endDate;
        }
    }
}