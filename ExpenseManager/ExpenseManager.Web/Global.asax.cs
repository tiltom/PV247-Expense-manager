using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using AutoMapper;
using ExpenseManager.Database;
using ExpenseManager.Database.Common;
using ExpenseManager.Entity.Budgets;
using ExpenseManager.Entity.Categories;
using ExpenseManager.Entity.Transactions;
using ExpenseManager.Entity.Wallets;
using ExpenseManager.Web.Binders;
using ExpenseManager.Web.Models.Budget;
using ExpenseManager.Web.Models.BudgetAccessRight;
using ExpenseManager.Web.Models.Category;
using ExpenseManager.Web.Models.Role;
using ExpenseManager.Web.Models.Transaction;
using ExpenseManager.Web.Models.Wallet;
using ExpenseManager.Web.Models.WalletAccessRight;

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
            MappingConfig.RegisterMappings();

            // Initialize Database
            RegisterContexts.Register();
            Database.Database.SetDatabaseInitializer();

            // Initialize auto mappings
            Mapper.CreateMap<Budget, BudgetShowModel>();
            Mapper.CreateMap<Budget, EditBudgetModel>();
            Mapper.CreateMap<Budget, NewBudgetModel>();

            Mapper.CreateMap<BudgetAccessRight, CreateBudgetAccessRightModel>();
            Mapper.CreateMap<BudgetAccessRight, EditBudgetAccessRightModel>();
            Mapper.CreateMap<BudgetAccessRight, ShowBudgetAccessRightModel>();

            Mapper.CreateMap<Category, CategoryShowModel>();

            Mapper.CreateMap<UserIdentity, RoleDetailViewModel>();
            Mapper.CreateMap<UserIdentity, RoleViewModel>();

            Mapper.CreateMap<Transaction, EditTransactionModel>();
            Mapper.CreateMap<Transaction, NewTransactionModel>();
            Mapper.CreateMap<Transaction, TransactionShowModel>();

            //map laso users??
            //Mapper.CreateMap<UserProfile, Web.Models.User.UserViewModel>();

            Mapper.CreateMap<Wallet, WalletEditModel>();

            Mapper.CreateMap<WalletAccessRight, WalletAccessRightModel>();

            // Model type binders
            ModelBinders.Binders.Add(typeof (decimal), new DecimalModelBinder());
        }
    }
}