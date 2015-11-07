using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using ExpenseManager.Web.Binders;
using ExpenseManager.Database;
using AutoMapper;
using ExpenseManager.Entity.Transactions;
using ExpenseManager.Entity.Budgets;
using ExpenseManager.Entity.Categories;
using ExpenseManager.Database.common;
using ExpenseManager.Entity.Users;
using ExpenseManager.Entity.Wallets;

namespace ExpenseManager.Web
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            // Initialize MVC
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Initialize Database
            RegisterContexts.Register();
            Database.Database.SetDatabaseInitializer();

            // Initialize auto mappings
            Mapper.CreateMap<Budget, Web.Models.Budget.BudgetShowModel>();
            Mapper.CreateMap<Budget, Web.Models.Budget.EditBudgetModel>();
            Mapper.CreateMap<Budget, Web.Models.Budget.NewBudgetModel>();

            Mapper.CreateMap<BudgetAccessRight, Web.Models.BudgetAccessRight.CreateBudgetAccessRightModel>();
            Mapper.CreateMap<BudgetAccessRight, Web.Models.BudgetAccessRight.EditBudgetAccessRightModel>();
            Mapper.CreateMap<BudgetAccessRight, Web.Models.BudgetAccessRight.ShowBudgetAccessRightModel>();

            Mapper.CreateMap<Category, Web.Models.Category.CategoryShowModel>();

            Mapper.CreateMap<UserIdentity, Web.Models.Role.RoleDetailViewModel>();
            Mapper.CreateMap<UserIdentity, Web.Models.Role.RoleViewModel>();

            Mapper.CreateMap<Transaction, Web.Models.Transaction.EditTransactionModel>();
            Mapper.CreateMap<Transaction, Web.Models.Transaction.NewTransactionModel>();
            Mapper.CreateMap<Transaction, Web.Models.Transaction.TransactionShowModel>();

            //map laso users??
            //Mapper.CreateMap<UserProfile, Web.Models.User.UserViewModel>();

            Mapper.CreateMap<Wallet, Web.Models.Wallet.WalletEditModel>();

            Mapper.CreateMap<WalletAccessRight, Web.Models.WalletAcessRight.WalletAcessRightModel>();

            // Model type binders
            ModelBinders.Binders.Add(typeof (decimal), new DecimalModelBinder());
        }
    }
}