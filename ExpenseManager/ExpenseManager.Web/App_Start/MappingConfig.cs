using System;
using AutoMapper;
using ExpenseManager.BusinessLogic.DashboardServices.Models;
using ExpenseManager.BusinessLogic.TransactionServices.Models;
using ExpenseManager.Database.Common;
using ExpenseManager.Entity.Budgets;
using ExpenseManager.Entity.Categories;
using ExpenseManager.Entity.Transactions;
using ExpenseManager.Entity.Wallets;
using ExpenseManager.Web.Models.Budget;
using ExpenseManager.Web.Models.BudgetAccessRight;
using ExpenseManager.Web.Models.Category;
using ExpenseManager.Web.Models.HomePage;
using ExpenseManager.Web.Models.Transaction;
using ExpenseManager.Web.Models.User;
using ExpenseManager.Web.Models.Wallet;
using ExpenseManager.Web.Models.WalletAccessRight;

namespace ExpenseManager.Web
{
    public static class MappingConfig
    {
        public static void RegisterMappings()
        {
            RegisterWalletAccessRightMappings();
            RegisterWalletMappings();
            RegisterUserIdentityMappings();
            RegisterBudgetAccessRightMappings();
            RegisterBudgetMappings();
            RegisterCategoryMappings();
            RegisterTransactionMappings();
            RegisterTransactionServiceMappings();
            RegisterDashBoardMappings();
        }

        private static void RegisterTransactionServiceMappings()
        {
            Mapper.CreateMap<Transaction, TransactionServiceModel>()
                .ForMember(dto => dto.Id, options => options.MapFrom(entity => entity.Guid))
                .ForMember(dto => dto.Expense, options => options.MapFrom(entity => entity.Amount < 0))
                .ForMember(dto => dto.Amount,
                    options => options.MapFrom(entity => entity.Amount < 0 ? entity.Amount*-1 : entity.Amount))
                .ForMember(dto => dto.Date, options => options.MapFrom(entity => entity.Date))
                .ForMember(dto => dto.Description, options => options.MapFrom(entity => entity.Description))
                .ForMember(dto => dto.WalletId, options => options.MapFrom(entity => entity.Wallet.Guid))
                .ForMember(dto => dto.BudgetId,
                    options => options.MapFrom(entity => entity.Budget == null ? Guid.Empty : entity.Budget.Guid))
                .ForMember(dto => dto.CurrencyId, options => options.MapFrom(entity => entity.Currency.Guid))
                .ForMember(dto => dto.CategoryId, options => options.MapFrom(entity => entity.Category.Guid));

            Mapper.CreateMap<Transaction, TransactionShowServiceModel>()
                .ForMember(dto => dto.Id, options => options.MapFrom(entity => entity.Guid))
                .ForMember(dto => dto.Amount, options => options.MapFrom(entity => entity.Amount))
                .ForMember(dto => dto.Date, options => options.MapFrom(entity => entity.Date))
                .ForMember(dto => dto.Description, options => options.MapFrom(entity => entity.Description))
                .ForMember(dto => dto.BudgetName,
                    options => options.MapFrom(entity => entity.Budget == null ? string.Empty : entity.Budget.Name))
                .ForMember(dto => dto.BudgetId,
                    options => options.MapFrom(entity => entity.Budget == null ? Guid.Empty : entity.Budget.Guid))
                .ForMember(dto => dto.CurrencySymbol, options => options.MapFrom(entity => entity.Currency.Symbol))
                .ForMember(dto => dto.CategoryName, options => options.MapFrom(entity => entity.Category.Name))
                .ForMember(dto => dto.CategoryId,
                    options => options.MapFrom(entity => entity.Category.Guid));
        }

        private static void RegisterTransactionMappings()
        {
            Mapper.CreateMap<Transaction, TransactionShowModel>()
                .ForMember(view => view.Id, options => options.MapFrom(entity => entity.Guid))
                .ForMember(view => view.Amount, options => options.MapFrom(entity => entity.Amount))
                .ForMember(view => view.Date, options => options.MapFrom(entity => entity.Date))
                .ForMember(view => view.Description, options => options.MapFrom(entity => entity.Description))
                .ForMember(view => view.CurrencySymbol, options => options.MapFrom(entity => entity.Currency.Symbol))
                .ForMember(view => view.CategoryName, options => options.MapFrom(entity => entity.Category.Name));

            Mapper.CreateMap<Transaction, EditTransactionModel>()
                .ForMember(view => view.Id, options => options.MapFrom(entity => entity.Guid))
                .ForMember(view => view.Amount, options => options.MapFrom(entity => entity.Amount))
                .ForMember(view => view.Date, options => options.MapFrom(entity => entity.Date))
                .ForMember(view => view.Description, options => options.MapFrom(entity => entity.Description))
                .ForMember(view => view.WalletId, options => options.MapFrom(entity => entity.Wallet.Guid))
                .ForMember(view => view.CurrencyId,
                    options => options.MapFrom(entity => entity.Currency.Guid.ToString()))
                .ForMember(view => view.CategoryId,
                    options => options.MapFrom(entity => entity.Category.Guid.ToString()));

            Mapper.CreateMap<NewTransactionModel, TransactionServiceModel>();

            Mapper.CreateMap<EditTransactionModel, TransactionServiceModel>();

            Mapper.CreateMap<TransactionServiceModel, EditTransactionModel>();

            Mapper.CreateMap<TransactionShowServiceModel, TransactionShowModel>();
        }

        private static void RegisterCategoryMappings()
        {
            Mapper.CreateMap<Category, CategoryShowModel>()
                .ForMember(view => view.Guid, options => options.MapFrom(entity => entity.Guid))
                .ForMember(view => view.Name, options => options.MapFrom(entity => entity.Name))
                .ForMember(view => view.Icon, options => options.MapFrom(entity => entity.IconPath))
                .ForMember(view => view.Type, options => options.MapFrom(entity => entity.Type))
                .ForMember(view => view.Description, options => options.MapFrom(entity => entity.Description));

            Mapper.CreateMap<CategoryShowModel, Category>()
                .ForMember(entity => entity.Guid, options => options.MapFrom(view => view.Guid))
                .ForMember(entity => entity.Name, options => options.MapFrom(view => view.Name))
                .ForMember(entity => entity.IconPath, options => options.MapFrom(view => view.Icon))
                .ForMember(entity => entity.Type, options => options.MapFrom(view => view.Type))
                .ForMember(entity => entity.Description, options => options.MapFrom(view => view.Description));
        }

        private static void RegisterBudgetMappings()
        {
            Mapper.CreateMap<Budget, BudgetShowModel>()
                .ForMember(view => view.Guid, options => options.MapFrom(entity => entity.Guid))
                .ForMember(view => view.Name, options => options.MapFrom(entity => entity.Name))
                .ForMember(view => view.StartDate, options => options.MapFrom(entity => entity.StartDate))
                .ForMember(view => view.EndDate, options => options.MapFrom(entity => entity.EndDate))
                .ForMember(view => view.Limit, options => options.MapFrom(entity => entity.Limit));

            Mapper.CreateMap<Budget, EditBudgetModel>()
                .ForMember(view => view.Guid, options => options.MapFrom(entity => entity.Guid))
                .ForMember(view => view.Name, options => options.MapFrom(entity => entity.Name))
                .ForMember(view => view.StartDate, options => options.MapFrom(entity => entity.StartDate))
                .ForMember(view => view.EndDate, options => options.MapFrom(entity => entity.EndDate))
                .ForMember(view => view.Limit, options => options.MapFrom(entity => entity.Limit));
        }

        private static void RegisterBudgetAccessRightMappings()
        {
            Mapper.CreateMap<BudgetAccessRight, EditBudgetAccessRightModel>()
                .ForMember(view => view.AssignedUserId, options => options.MapFrom(entity => entity.UserProfile.Guid))
                .ForMember(view => view.AssignedUserName,
                    options =>
                        options.MapFrom(entity => entity.UserProfile.FirstName + " " + entity.UserProfile.LastName))
                .ForMember(view => view.Permission, options => options.MapFrom(entity => entity.Permission))
                .ForMember(view => view.BudgetId, options => options.MapFrom(entity => entity.Budget.Guid))
                .ForMember(view => view.Id, options => options.MapFrom(entity => entity.Guid));

            Mapper.CreateMap<BudgetAccessRight, ShowBudgetAccessRightModel>()
                .ForMember(view => view.AssignedUserName,
                    options =>
                        options.MapFrom(entity => entity.UserProfile.FirstName + " " + entity.UserProfile.LastName))
                .ForMember(view => view.Id, options => options.MapFrom(entity => entity.Guid))
                .ForMember(view => view.Permission, options => options.MapFrom(entity => entity.Permission));
        }

        private static void RegisterUserIdentityMappings()
        {
            Mapper.CreateMap<UserIdentity, UserViewModel>()
                .ForMember(view => view.Id, options => options.MapFrom(entity => entity.Id))
                .ForMember(view => view.Email, options => options.MapFrom(entity => entity.Email))
                .ForMember(view => view.FirstName, options => options.MapFrom(entity => entity.Profile.FirstName))
                .ForMember(view => view.LastName, options => options.MapFrom(entity => entity.Profile.LastName));

            Mapper.CreateMap<UserIdentity, UserDetailViewModel>()
                .ForMember(view => view.Id, options => options.MapFrom(entity => entity.Id))
                .ForMember(view => view.Email, options => options.MapFrom(entity => entity.Email))
                .ForMember(view => view.UserName,
                    options => options.MapFrom(entity => entity.Profile.FirstName + " " + entity.Profile.LastName));

            Mapper.CreateMap<UserIdentity, UserEditViewModel>()
                .ForMember(view => view.Id, options => options.MapFrom(entity => entity.Id))
                .ForMember(view => view.Email, options => options.MapFrom(entity => entity.Email))
                .ForMember(view => view.FirstName, options => options.MapFrom(entity => entity.Profile.FirstName))
                .ForMember(view => view.LastName, options => options.MapFrom(entity => entity.Profile.LastName));
        }

        private static void RegisterWalletMappings()
        {
            Mapper.CreateMap<Wallet, WalletEditModel>()
                .ForMember(view => view.Guid, options => options.MapFrom(entity => entity.Guid))
                .ForMember(view => view.CurrencyId, options => options.MapFrom(entity => entity.Currency.Guid))
                .ForMember(view => view.Name, options => options.MapFrom(entity => entity.Name));
        }

        private static void RegisterWalletAccessRightMappings()
        {
            Mapper.CreateMap<WalletAccessRight, WalletAccessRightModel>()
                .ForMember(view => view.Id, options => options.MapFrom(entity => entity.Guid))
                .ForMember(view => view.Permission, options => options.MapFrom(entity => entity.Permission.ToString()))
                .ForMember(view => view.AssignedUserName,
                    options =>
                        options.MapFrom(entity => entity.UserProfile.FirstName + " " + entity.UserProfile.LastName))
                .ForMember(view => view.WalletId, options => options.MapFrom(entity => entity.Wallet.Guid));

            Mapper.CreateMap<WalletAccessRight, WalletAccessRightEditModel>()
                .ForMember(view => view.Id, options => options.MapFrom(entity => entity.Guid))
                .ForMember(view => view.Permission, options => options.MapFrom(entity => entity.Permission.ToString()))
                .ForMember(view => view.AssignedUserId, options => options.MapFrom(entity => entity.UserProfile.Guid))
                .ForMember(view => view.AssignedUserName,
                    options =>
                        options.MapFrom(entity => entity.UserProfile.FirstName + " " + entity.UserProfile.LastName))
                .ForMember(view => view.WalletId, options => options.MapFrom(entity => entity.Wallet.Guid));
        }

        private static void RegisterDashBoardMappings()
        {
            Mapper.CreateMap<FilterDataModel, FilterDataServiceModel>()
                .ForMember(view => view.Categories, options => options.MapFrom(model => model.Categories))
                .ForMember(view => view.Wallets, options => options.MapFrom(model => model.Wallets))
                .ForMember(view => view.Budgets, options => options.MapFrom(model => model.Budgets));

            Mapper.CreateMap<FilterDataServiceModel, FilterDataModel>()
                .ForMember(view => view.Categories, options => options.MapFrom(model => model.Categories))
                .ForMember(view => view.Wallets, options => options.MapFrom(model => model.Wallets))
                .ForMember(view => view.Budgets, options => options.MapFrom(model => model.Budgets));

            Mapper.CreateMap<DashBoardServiceModel, DashBoardModel>()
                .ForMember(
                    view => view.CategoriesExpenseChart,
                    options => options.MapFrom(model => model.CategoriesExpenseChart)
                )
                .ForMember(view => view.MonthSummaryChart, options => options.MapFrom(model => model.MonthSummaryChart))
                .ForMember(
                    view => view.CategoriesIncomeChart,
                    options => options.MapFrom(model => model.CategoriesIncomeChart)
                )
                .ForMember(view => view.Transactions, options => options.MapFrom(model => model.Transactions))
                .ForMember(view => view.YearSummaryChart, options => options.MapFrom(model => model.YearSummaryChart));
        }
    }
}