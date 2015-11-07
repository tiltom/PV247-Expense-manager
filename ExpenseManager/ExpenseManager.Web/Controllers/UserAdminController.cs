using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ExpenseManager.Entity.Users;
using ExpenseManager.Entity.Wallets;
using ExpenseManager.Web.Common;
using ExpenseManager.Web.Helpers;
using ExpenseManager.Web.Models.User;
using Microsoft.AspNet.Identity.Owin;
using WebGrease.Css.Extensions;
using ExpenseManager.Database.common;

namespace ExpenseManager.Web.Controllers
{
    [Authorize(Roles = "Admin")]
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
        public ActionResult Index()
        {
            return this.View(UserManager.Users.Select(u => new UserViewModel
            {
                Id = u.Id,
                Email = u.Email,
                FirstName = u.Profile.FirstName,
                LastName = u.Profile.LastName
            }));
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
            var roleNames = await UserManager.GetRolesAsync(user.Id);

            return this.View(new UserDetailViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.Profile.FirstName + " " + user.Profile.LastName,
                RolesList = roleNames
            });
        }

        /// <summary>
        ///     Display create user form
        /// </summary>
        /// <returns>View</returns>
        public async Task<ActionResult> Create()
        {
            //Get the list of SelectedRoles
            return this.View(new RegisterViewModel
            {
                RolesList = await this.GetAllRolesAsync()
            });
        }

        private Task<List<SelectListItem>> GetAllRolesAsync()
        {
            return RoleManager.Roles.Select(
                r => new SelectListItem {Value = r.Id, Text = r.Name})
                .ToListAsync();
        }

        /// <summary>
        ///     Create user
        /// </summary>
        /// <param name="userViewModel">RegisterViewModel instance</param>
        /// <returns>View</returns>
        [HttpPost]
        public async Task<ActionResult> Create(RegisterViewModel userViewModel)
        {
            if (!ModelState.IsValid)
            {
                return this.View(userViewModel);
            }

            if (userViewModel.SelectedRoles == null)
            {
                ModelState.AddModelError("", "At least one role has to be selected.");
                userViewModel.RolesList = await this.GetAllRolesAsync();
                return this.View(userViewModel);
            }

            var user = new UserIdentity
            {
                UserName = userViewModel.Email,
                Email = userViewModel.Email,
                CreationDate = DateTime.Now,
                Profile = new UserProfile
                {
                    FirstName = userViewModel.FirstName,
                    LastName = userViewModel.LastName,
                    PersonalWallet = new Wallet
                    {
                        Name = "Default Wallet",
                        Currency = await this.GetDefaultCurrency()
                    }
                }
            };
            var adminresult = await UserManager.CreateAsync(user, userViewModel.Password);


            //Add User to the selected SelectedRoles 
            if (adminresult.Succeeded)
            {
                if (userViewModel.SelectedRoles == null) return this.RedirectToAction("Index");

                var result = await UserManager.AddToRolesAsync(user.Id, userViewModel.SelectedRoles.ToArray());
                if (!result.Succeeded)
                {
                    result.Errors.ForEach(e => ModelState.AddModelError("", e));
                    userViewModel.RolesList = await this.GetAllRolesAsync();
                    return this.View(userViewModel);
                }
            }
            else
            {
                adminresult.Errors.ForEach(e => ModelState.AddModelError("", e));
                userViewModel.RolesList = await this.GetAllRolesAsync();
                return this.View(userViewModel);
            }
            return this.RedirectToAction("Index");
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

            var userRoles = await UserManager.GetRolesAsync(user.Id);

            return this.View(new UserEditViewModel
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.Profile.FirstName,
                LastName = user.Profile.LastName,
                RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem
                {
                    Selected = userRoles.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Name
                })
            });
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
                ModelState.AddModelError("", "Something failed.");
                return this.View();
            }

            if (editUser.SelectedRoles == null)
            {
                ModelState.AddModelError("", "At least one role has to be selected.");
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
                result.Errors.ForEach(e => ModelState.AddModelError("", e));
                return this.View(editUser);
            }
            result = await UserManager.RemoveFromRolesAsync(user.Id, userRoles.Except(selectedRoles).ToArray());

            if (!result.Succeeded)
            {
                result.Errors.ForEach(e => ModelState.AddModelError("", e));
                return this.View(editUser);
            }
            return this.RedirectToAction("Index");
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
            if (!ModelState.IsValid) return this.View();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return this.HttpNotFound();
            }
            var result = await UserManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                result.Errors.ForEach(e => ModelState.AddModelError("", e));
                return this.View();
            }
            return this.RedirectToAction("Index");
        }
    }
}