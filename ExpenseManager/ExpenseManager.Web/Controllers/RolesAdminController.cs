using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ExpenseManager.Database;
using ExpenseManager.Web.Constants;
using ExpenseManager.Web.Constants.UserConstants;
using ExpenseManager.Web.Helpers;
using ExpenseManager.Web.Models.Role;
using ExpenseManager.Web.Models.User;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using PagedList;
using WebGrease.Css.Extensions;

namespace ExpenseManager.Web.Controllers
{
    [Authorize(Roles = UserIdentity.AdminRole)]
    public class RolesAdminController : Controller
    {
        private ApplicationRoleManager _roleManager;

        private ApplicationUserManager _userManager;

        public RolesAdminController()
        {
        }

        public RolesAdminController(ApplicationUserManager userManager,
            ApplicationRoleManager roleManager)
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
        ///     Display list of available roles
        /// </summary>
        /// <returns>View</returns>
        public ActionResult Index(int? page)
        {
            var pageSize = SharedConstant.PageSize;
            var pageNumber = (page ?? SharedConstant.DefaultStartPage);

            return this.View(RoleManager.Roles.OrderBy(x => x.Name).ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        ///     Show details for selected role
        /// </summary>
        /// <param name="id">Guid of selected role which will be looked in database</param>
        /// <returns></returns>
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var role = await RoleManager.FindByIdAsync(id);
            // Get the list of Users in this Role
            var users = new List<UserIdentity>();

            // Get the list of Users in this Role
            foreach (var user in UserManager.Users.ToList())
            {
                if (await UserManager.IsInRoleAsync(user.Id, role.Name))
                {
                    users.Add(user);
                }
            }

            return this.View(new RoleDetailViewModel
            {
                Name = role.Name,
                Users = users.Select(user => new UserDetailViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName
                }).ToList()
            });
        }

        /// <summary>
        ///     Display create form for new role
        /// </summary>
        /// <returns>View</returns>
        public ActionResult Create()
        {
            return this.View();
        }

        /// <summary>
        ///     Create new role
        /// </summary>
        /// <param name="roleViewModel">RoleViewModel instance</param>
        /// <returns>View</returns>
        [HttpPost]
        public async Task<ActionResult> Create(RoleViewModel roleViewModel)
        {
            if (ModelState.IsValid)
            {
                var role = new IdentityRole(roleViewModel.Name);
                var roleresult = await RoleManager.CreateAsync(role);

                if (roleresult.Succeeded) return this.RedirectToAction(SharedConstant.Index);

                roleresult.Errors.ForEach(error => ModelState.AddModelError("", error));
                return this.View();
            }
            return this.View();
        }

        /// <summary>
        ///     Display edit form for selected role
        /// </summary>
        /// <param name="id">id of selected role</param>
        /// <returns></returns>
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var role = await RoleManager.FindByIdAsync(id);
            if (role == null)
            {
                return this.HttpNotFound();
            }

            var roleModel = new RoleViewModel
            {
                Id = role.Id,
                Name = role.Name
            };

            return this.View(roleModel);
        }

        /// <summary>
        ///     Edit selected role
        /// </summary>
        /// <param name="roleModel">RoleViewModel instance</param>
        /// <returns>View</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = UserConstant.EditRoleInclude)] RoleViewModel roleModel)
        {
            if (ModelState.IsValid)
            {
                var role = await RoleManager.FindByIdAsync(roleModel.Id);
                role.Name = roleModel.Name;
                await RoleManager.UpdateAsync(role);
                return this.RedirectToAction(SharedConstant.Index);
            }
            return this.View();
        }

        /// <summary>
        ///     Display delete dialog for selected role
        /// </summary>
        /// <param name="id">id of selected role</param>
        /// <returns>View</returns>
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var role = await RoleManager.FindByIdAsync(id);
            if (role == null)
            {
                return this.HttpNotFound();
            }

            return this.View(role);
        }

        /// <summary>
        ///     Delete selected role
        /// </summary>
        /// <param name="id">id of selected role</param>
        /// <returns></returns>
        [HttpPost, ActionName(SharedConstant.Delete)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var role = await RoleManager.FindByIdAsync(id);
                if (role == null)
                {
                    return this.HttpNotFound();
                }

                var usersInRole =
                    UserManager.Users.Where(user => user.Roles.Select(userRole => userRole.RoleId).Contains(role.Id))
                        .ToList();

                foreach (var user in usersInRole)
                {
                    UserManager.RemoveFromRoles(user.Id, role.Name);
                }

                var result = await RoleManager.DeleteAsync(role);

                if (!result.Succeeded)
                {
                    result.Errors.ForEach(error => ModelState.AddModelError("", error));
                    return this.View();
                }
                return this.RedirectToAction(SharedConstant.Index);
            }
            return this.View();
        }
    }
}