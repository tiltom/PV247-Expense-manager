using System.Collections.Generic;
using System.Data.Entity;
using ExpenseManager.Entity;

namespace ExpenseManager.Web.DatabaseContexts
{
    public class TransactionInitializater : DropCreateDatabaseIfModelChanges<TransactionDbContext>
    {
        protected override void Seed(TransactionDbContext context)
        {
            var categories = new List<Category>
            {
                new Category
                {
                    Name = "Other",
                    Description = "Category for non-classifiable transactions",
                    IconPath = "glyphicons-circle-question-mark"
                },
                new Category
                {
                    Name = "Food & Drinks",
                    Description = "Category for comsumables",
                    IconPath = "glyphicons-fast-food"
                },
                new Category
                {
                    Name = "Travel",
                    Description = "Category for transportation and related stuff",
                    IconPath = "glyphicons-transport"
                }
            };
            context.Categories.AddRange(categories);

            context.SaveChanges();
        }
    }
}