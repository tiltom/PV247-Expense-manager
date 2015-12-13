using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using ExpenseManager.Database;
using ExpenseManager.Entity;
using ExpenseManager.Entity.Providers.Factory;
using ExpenseManager.Entity.Users;
using ExpenseManager.Resources;
using ExpenseManager.Resources.UsersAdminResources;
using ExpenseManager.Web.Constants;
using ExpenseManager.Web.Helpers;
using ExpenseManager.Web.Models.User;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PagedList;
using WebGrease.Css.Extensions;
using QueryableExtensions = System.Data.Entity.QueryableExtensions;

namespace ExpenseManager.Web.Controllers
{
    [Authorize(Roles = UserIdentity.AdminRole)]
    public class UsersAdminController : AbstractController
    {
        private ApplicationRoleManager _roleManager;

        private ApplicationUserManager _userManager;

        public UsersAdminController()
        {
        }

        public UsersAdminController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return this._userManager ??
                       (this._userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>());
            }
            private set { this._userManager = value; }
        }

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return this._roleManager ??
                       (this._roleManager = HttpContext.GetOwinContext().Get<ApplicationRoleManager>());
            }
            private set { this._roleManager = value; }
        }

        /// <summary>
        ///     Display all user accounts
        /// </summary>
        /// <returns>View</returns>
        public ActionResult Index(int? page)
        {
            var pageSize = SharedConstant.PageSize;
            var pageNumber = (page ?? SharedConstant.DefaultStartPage);

            return
                this.View(
                    Mapper.Map<IEnumerable<UserViewModel>>(
                        UserManager.Users.ToList().Where(u => u.Id != User.Identity.GetUserId()))
                        .ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        ///     Display user detail for selected user
        /// </summary>
        /// <param name="id">id of selected user</param>
        /// <returns>View</returns>
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            var userDetailViewModel = Mapper.Map<UserDetailViewModel>(user);
            userDetailViewModel.RolesList = await UserManager.GetRolesAsync(user.Id);

            return this.View(userDetailViewModel);
        }

        /// <summary>
        ///     Display create user form
        /// </summary>
        /// <returns>View</returns>
        public async Task<ActionResult> Create()
        {
            //Get the list of SelectedRoles
            return this.View(new RegisterWithPasswordViewModel
            {
                RolesList = await this.GetAllRolesAsync(),
                Currencies = await this.GetCurrencies()
            });
        }

        private Task<List<SelectListItem>> GetAllRolesAsync()
        {
            return QueryableExtensions.ToListAsync(RoleManager.Roles.Select(
                role => new SelectListItem {Value = role.Id, Text = role.Name}));
        }

        /// <summary>
        ///     Create user
        /// </summary>
        /// <param name="userWithPasswordViewModel">RegisterWithPasswordViewModel instance</param>
        /// <returns>View</returns>
        [HttpPost]
        public async Task<ActionResult> Create(RegisterWithPasswordViewModel userWithPasswordViewModel)
        {
            if (!ModelState.IsValid)
            {
                this.AddError(SharedResource.ModelStateIsNotValid);
                userWithPasswordViewModel.RolesList = await this.GetAllRolesAsync();
                userWithPasswordViewModel.Currencies = await this.GetCurrencies();
                return this.View(userWithPasswordViewModel);
            }

            if (userWithPasswordViewModel.SelectedRoles == null)
            {
                ModelState.AddModelError("", UsersAdminResource.OneRoleMustBeSelected);
                userWithPasswordViewModel.RolesList = await this.GetAllRolesAsync();
                userWithPasswordViewModel.Currencies = await this.GetCurrencies();
                return this.View(userWithPasswordViewModel);
            }

            var user = await this.CreateUser(userWithPasswordViewModel);
            var adminresult = await UserManager.CreateAsync(user, userWithPasswordViewModel.Password);


            //Add User to the selected SelectedRoles 
            if (adminresult.Succeeded)
            {
                if (userWithPasswordViewModel.SelectedRoles == null) return this.RedirectToAction(SharedConstant.Index);

                var result =
                    await UserManager.AddToRolesAsync(user.Id, userWithPasswordViewModel.SelectedRoles.ToArray());
                if (!result.Succeeded)
                {
                    result.Errors.ForEach(error => ModelState.AddModelError("", error));
                    userWithPasswordViewModel.RolesList = await this.GetAllRolesAsync();
                    return this.View(userWithPasswordViewModel);
                }
            }
            else
            {
                adminresult.Errors.ForEach(e => ModelState.AddModelError("", e));
                userWithPasswordViewModel.RolesList = await this.GetAllRolesAsync();
                userWithPasswordViewModel.Currencies = await this.GetCurrencies();
                return this.View(userWithPasswordViewModel);
            }
            return this.RedirectToAction(SharedConstant.Index);
        }

        /// <summary>
        ///     Display edit form for selected user
        /// </summary>
        /// <param name="id">id of selected user</param>
        /// <returns>View</returns>
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return this.HttpNotFound();
            }

            var userEditModel = Mapper.Map<UserEditViewModel>(user);
            var userRoles = await UserManager.GetRolesAsync(user.Id);
            userEditModel.RolesList = RoleManager.Roles.ToList().Select(role => new SelectListItem
            {
                Selected = userRoles.Contains(role.Name),
                Text = role.Name,
                Value = role.Name
            });


            return this.View(userEditModel);
        }

        /// <summary>
        ///     Edit selected user
        /// </summary>
        /// <param name="editUser">EditUserViewModel instance</param>
        /// <returns>View</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UserEditViewModel editUser)
        {
            if (!ModelState.IsValid)
            {
                this.AddError(SharedResource.ModelStateIsNotValid);
                return this.View();
            }

            if (editUser.SelectedRoles == null)
            {
                ModelState.AddModelError("", UsersAdminResource.OneRoleMustBeSelected);
                editUser.RolesList = await this.GetAllRolesAsync();
                return this.View(editUser);
            }

            var user = await UserManager.FindByIdAsync(editUser.Id);
            if (user == null)
            {
                return this.HttpNotFound();
            }

            user.UserName = editUser.Email;
            user.Email = editUser.Email;
            user.Profile.FirstName = editUser.FirstName;
            user.Profile.LastName = editUser.LastName;

            var userRoles = await UserManager.GetRolesAsync(user.Id);
            var selectedRoles = editUser.SelectedRoles?.ToArray() ?? new string[] {};
            var result = await UserManager.AddToRolesAsync(user.Id, selectedRoles.Except(userRoles).ToArray());

            if (!result.Succeeded)
            {
                result.Errors.ForEach(error => ModelState.AddModelError("", error));
                return this.View(editUser);
            }
            result = await UserManager.RemoveFromRolesAsync(user.Id, userRoles.Except(selectedRoles).ToArray());

            if (!result.Succeeded)
            {
                result.Errors.ForEach(error => ModelState.AddModelError("", error));
                return this.View(editUser);
            }
            return this.RedirectToAction(SharedConstant.Index);
        }

        /// <summary>
        ///     Display delete view for selected user
        /// </summary>
        /// <param name="id">id of selected user</param>
        /// <returns>View</returns>
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return this.HttpNotFound();
            }
            return this.View(user);
        }

        /// <summary>
        ///     Delete selected user
        /// </summary>
        /// <param name="id">id of selected user</param>
        /// <returns>View</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (!ModelState.IsValid)
            {
                this.AddError(SharedResource.ModelStateIsNotValid);
                return this.View();
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return this.HttpNotFound();
            }

            var profile = user.Profile;
            await this.DeleteUserDependentEntities(profile);
            var result = await UserManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                result.Errors.ForEach(error => ModelState.AddModelError("", error));
                return this.View();
            }

            var budgetProvider = ProvidersFactory.GetNewBudgetsProviders();
            await budgetProvider.DeteleAsync(profile);

            return this.RedirectToAction(SharedConstant.Index);
        }

        private async Task DeleteUserDependentEntities(UserProfile profile)
        {
            // Delete WalletAccessRights, Wallet and Transactions connected with Wallet
            var walletToDelete =
                profile.WalletAccessRights.FirstOrDefault(right => right.Permission == PermissionEnum.Owner)?.Wallet;
            var rightToDelete = walletToDelete?.WalletAccessRights.ToList();
            var transactionsToDelete = walletToDelete?.Transactions.ToList();

            var transactionsProvider = ProvidersFactory.GetNewTransactionsProviders();
            if (transactionsToDelete != null)
                foreach (var transaction in transactionsToDelete)
                {
                    await transactionsProvider.DeteleAsync(transaction);
                }

            var walletProvider = ProvidersFactory.GetNewWalletsProviders();
            if (rightToDelete != null)
                foreach (var walletAccessRight in rightToDelete)
                {
                    await walletProvider.DeteleAsync(walletAccessRight);
                }
            await walletProvider.DeteleAsync(walletToDelete);

            // Delete BudgetAccessRights and CreatedBudgets
            var budgetProvider = ProvidersFactory.GetNewBudgetsProviders();
            var userBudgets =
                profile.BudgetAccessRights
                    .Where(right => right.Permission == PermissionEnum.Owner)
                    .Select(right => right.Budget);
            foreach (var createdBudget in userBudgets)
            {
                await budgetProvider.DeteleAsync(createdBudget);
            }
        }
    }
}