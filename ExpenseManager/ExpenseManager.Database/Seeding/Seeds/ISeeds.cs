using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseManager.Database.Seeding.Seeds
{
    internal interface ISeeds<TContext>
        where TContext : DbContext
    {
        void Seed(TContext context);
    }
}
