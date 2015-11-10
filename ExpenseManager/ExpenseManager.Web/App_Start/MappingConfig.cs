using AutoMapper;
using ExpenseManager.Entity.Budgets;
using ExpenseManager.Entity.Categories;
using ExpenseManager.Entity.Transactions;
using ExpenseManager.Entity.Wallets;
using ExpenseManager.Web.Common;
using ExpenseManager.Web.Models.Budget;
using ExpenseManager.Web.Models.BudgetAccessRight;
using ExpenseManager.Web.Models.Category;
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
            Mapper.CreateMap<WalletAccessRight, WalletAccessRightModel>()
                .ForMember(view => view.Id, options => options.MapFrom(entity => entity.Guid))
                .ForMember(view => view.Permission, options => options.MapFrom(entity => entity.Permission.ToString()))
                .ForMember(view => view.AssignedUserId, options => options.MapFrom(entity => entity.UserProfile.Guid))
                .ForMember(view => view.AssignedUserName,
                    options =>
                        options.MapFrom(entity => entity.UserProfile.FirstName + " " + entity.UserProfile.LastName))
                .ForMember(view => view.WalletId, options => options.MapFrom(entity => entity.Wallet.Guid));

            Mapper.CreateMap<Wallet, WalletEditModel>()
                .ForMember(view => view.Guid, options => options.MapFrom(entity => entity.Guid))
                .ForMember(view => view.CurrencyId, options => options.MapFrom(entity => entity.Currency.Guid))
                .ForMember(view => view.Name, options => options.MapFrom(entity => entity.Name));

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

            Mapper.CreateMap<Budget, BudgetShowModel>()
                .ForMember(view => view.Guid, options => options.MapFrom(entity => entity.Guid))
                .ForMember(view => view.Name, options => options.MapFrom(entity => entity.Name))
                .ForMember(view => view.StartDate, options => options.MapFrom(entity => entity.StartDate))
                .ForMember(view => view.StartDate, options => options.MapFrom(entity => entity.EndDate))
                .ForMember(view => view.Limit, options => options.MapFrom(entity => entity.Limit));

            Mapper.CreateMap<Budget, EditBudgetModel>()
                .ForMember(view => view.Guid, options => options.MapFrom(entity => entity.Guid))
                .ForMember(view => view.Name, options => options.MapFrom(entity => entity.Name))
                .ForMember(view => view.StartDate, options => options.MapFrom(entity => entity.StartDate))
                .ForMember(view => view.StartDate, options => options.MapFrom(entity => entity.EndDate))
                .ForMember(view => view.Limit, options => options.MapFrom(entity => entity.Limit));

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
        }
    }
}