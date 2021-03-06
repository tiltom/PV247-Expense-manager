﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CaptchaMvc.HtmlHelpers;
using ExpenseManager.BusinessLogic;
using ExpenseManager.BusinessLogic.BudgetServices;
using ExpenseManager.Entity.Providers.Factory;
using ExpenseManager.Resources;
using ExpenseManager.Resources.BudgetResources;
using ExpenseManager.Web.Constants;
using ExpenseManager.Web.Helpers;
using ExpenseManager.Web.Models.BudgetAccessRight;
using PagedList;
using System.Net.Mail;
using System.Net;
using ExpenseManager.Entity.Budgets;
using System.Configuration;
using ExpenseManager.Web.Constants.LoginConstants;
using ExpenseManager.Web.Constants.RegistrationConstants;
using ExpenseManager.Web.Constants.BudgetAccessRightConstants;

namespace ExpenseManager.Web.Controllers
{
    [Authorize]
    public class BudgetAccessRightController : AbstractController
    {
        private readonly BudgetAccessRightService _budgetAccessRightService
            = new BudgetAccessRightService(ProvidersFactory.GetNewBudgetsProviders());

        private readonly BudgetService _budgetService =
            new BudgetService(ProvidersFactory.GetNewBudgetsProviders(), ProvidersFactory.GetNewTransactionsProviders());

        /// <summary>
        ///     Display all budget access rights for chosen budget
        /// </summary>
        /// <param name="id">Id of chosen budget</param>
        /// <param name="page">Number of page which user wants to see</param>
        /// <returns>View with model</returns>
        public async Task<ActionResult> Index(Guid id, int? page)
        {
            var accessRightModels =
                await
                    this._budgetAccessRightService.GetAccessRightsByBudgetId(id)
                        .ProjectTo<ShowBudgetAccessRightModel>()
                        .ToListAsync();

            accessRightModels.ForEach(model => model.BudgetId = id);
            var pageNumber = page ?? SharedConstant.DefaultStartPage;
            return this.View(accessRightModels.ToPagedList(pageNumber, SharedConstant.PageSize));
        }

        /// <summary>
        ///     Create new budget access right
        /// </summary>
        /// <param name="id">Id of budget where budget access right belongs</param>
        /// <returns></returns>
        public ActionResult Create(Guid id)
        {
            // creating new CreateBudgetAccessRightModel instance
            var model = new CreateBudgetAccessRightModel
            {
                BudgetId = id,
                Permissions = this.GetPermissions()
            };

            return this.View(model);
        }

        /// <summary>
        ///     Confirm budgetAccessRight when user guid is known, otherwise redirects user to registration.
        /// </summary>
        /// <param name="budgetId">Budget id</param>
        /// <param name="userEmail">User email</param>
        /// <param name="permission">Permission</param>
        /// <param name="hash">Hash counted from application id, budget id, user email and permission</param>
        /// <returns>Redirect to Index when user was created. To registration otherwise.</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmRequest(Guid budgetId, string userEmail, Entity.PermissionEnum permission, int hash)
        {
            // Check if hash is correct
            string stringToCheck = getConfirmationString(new CreateBudgetAccessRightModel { BudgetId = budgetId, AssignedUserEmail = userEmail, Permission = permission });
            if (stringToCheck.GetHashCode() != hash)
            {
                this.AddError(string.Format(BudgetAccessRightResource.InvalidLink));
                return this.RedirectToAction(SharedConstant.Index, SharedConstant.DashBoard);
            }

            Guid userId = await GetUserProfileByEmail(userEmail);
            var user = await UserContext.UserProfiles.Where(u => u.Guid == userId).FirstOrDefaultAsync();

            if (user != null)
            {
                if (userId != await CurrentProfileId())
                    return this.RedirectToAction(LoginConstant.Login, SharedConstant.Account, new { ReturnUrl = Request.RawUrl });

                bool added = await this._budgetAccessRightService.CreateBudgetAccessRight(budgetId, userId, permission);
                if (!added)
                {
                    this.AddError(BudgetAccessRightResource.UnsuccessfulBudgetAccessRightAdding);
                }
                else
                {
                    Budget budget = await this._budgetService.GetBudgetById(budgetId);
                    this.AddSuccess(string.Format(BudgetAccessRightResource.SuccessfullCreation, permission, budget.Name));
                }
                return this.RedirectToAction(SharedConstant.Index, SharedConstant.DashBoard);
            }
            else
            {
                return this.RedirectToAction(RegisterConstant.Register, SharedConstant.Account, new { ReturnUrl = Request.RawUrl });
            }
        }

        /// <summary>
        ///     Send request for budget access right
        /// </summary>
        /// <param name="model">CreateBudgetAccessRightModel instance</param>
        /// <returns>Redirect to Index</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateBudgetAccessRightModel model)
        {
            this.IsCaptchaValid(SharedResource.CaptchaValidationFailed);
            // checking if model is valid
            if (!ModelState.IsValid)
            {
                this.AddError(SharedResource.ModelStateIsNotValid);
                model.Permissions = this.GetPermissions();
                return this.View(model);
            }

            var userId = await this.GetUserProfileByEmail(model.AssignedUserEmail);
            try
            {
                await SendRequest(model);
                this.AddSuccess(string.Format(BudgetAccessRightResource.RequestSent, model.AssignedUserEmail));
                return RedirectToAction(SharedConstant.Index, new { id = model.BudgetId });
            }
            catch (ServiceValidationException exception)
            {
                ModelState.AddModelErrors(exception);

                model.Permissions = this.GetPermissions();
                return this.View(model);
            }
        }

        /// <summary>
        /// Sends email to user with link which can be used to share desired budget.
        /// </summary>
        /// <param name="model">budget access right model with set id, email and permission</param>
        private async Task SendRequest(CreateBudgetAccessRightModel model)
        {
            var confirmationData = getConfirmationString(model);
            var callbackUrl = Url.Action(BudgetAccessRightConstant.ConfirmRequest, BudgetAccessRightConstant.BudgetAccessRight, 
                new { budgetId = model.BudgetId, userEmail = model.AssignedUserEmail, permission = model.Permission, hash=confirmationData.GetHashCode() }, protocol: Request.Url.Scheme);
            var message = new MailMessage();
            message.To.Add(new MailAddress(model.AssignedUserEmail));
            message.Subject = BudgetAccessRightResource.EmailSubject;
            message.Body = string.Format(BudgetAccessRightResource.EmailInvitation, User.Identity.Name, callbackUrl.ToString());
            message.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = ConfigurationManager.AppSettings["GoogleUserName"],
                    Password = ConfigurationManager.AppSettings["GooglePassword"]
                };
                smtp.Credentials = credential;
                await smtp.SendMailAsync(message);
            }
        }

        /// <summary>
        ///     Edit chosen budget access right
        /// </summary>
        /// <param name="id">Id of budget where the budget access right belongs</param>
        /// <returns>View with model</returns>
        public async Task<ActionResult> Edit(Guid id)
        {
            // find BudgetAccessRight by its Id
            var budgetAccessRight = await this._budgetAccessRightService.GetBudgetAccessRightById(id);

            // creating EditBudgetAccessRight model instance from BudgetAccessRight DB entity
            var model = Mapper.Map<EditBudgetAccessRightModel>(budgetAccessRight);
            model.Permissions = this.GetPermissions();

            return this.View(model);
        }

        /// <summary>
        ///     Edit chosen budget access right
        /// </summary>
        /// <param name="model">EditBudgetAccessRightModel instance</param>
        /// <returns>Redirect to Index</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditBudgetAccessRightModel model)
        {
            // checking if model is valid
            if (!ModelState.IsValid)
            {
                this.AddError(SharedResource.ModelStateIsNotValid);
                return this.View(model);
            }

            try
            {
                await
                    this._budgetAccessRightService.EditBudgetAccessRight(model.Id, model.Permission,
                        model.AssignedUserId);
            }
            catch (ServiceValidationException exception)
            {
                ModelState.AddModelErrors(exception);
                model.Permissions = this.GetPermissions();
                return this.View(model);
            }

            this.AddSuccess(string.Format(BudgetAccessRightResource.SuccessfullEdit, model.AssignedUserName));
            return this.RedirectToAction(SharedConstant.Index, new { id = model.BudgetId });
        }

        /// <summary>
        ///     Method for displaying view with confirmation of deleting budget access right.
        /// </summary>
        /// <param name="id">id of budget access right to delete</param>
        /// <returns></returns>
        public async Task<ActionResult> Delete(Guid id, Guid budgetId)
        {
            // find BudgetAccessRight by its Id
            var budgetAccessRight = await this._budgetAccessRightService.GetBudgetAccessRightById(id);

            var model = Mapper.Map<ShowBudgetAccessRightModel>(budgetAccessRight);
            model.BudgetId = budgetId;

            return this.View(model);
        }

        /// <summary>
        ///     Delete budget access right from DB
        /// </summary>
        /// <param name="model">ShowBudgetAccessRightModel of budget access right to delete</param>
        /// <returns>Redirect to Index</returns>
        [HttpPost, ActionName(SharedConstant.Delete)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(ShowBudgetAccessRightModel model)
        {
            if (!ModelState.IsValid)
            {
                this.AddError(SharedResource.ModelStateIsNotValid);
                return this.RedirectToAction(SharedConstant.Index, new { id = model.BudgetId });
            }

            await this._budgetAccessRightService.DeleteBudgetAccessRight(model.Id);

            this.AddSuccess(string.Format(BudgetAccessRightResource.SuccessfullDelete, model.Permission,
                model.AssignedUserName));
            return this.RedirectToAction(SharedConstant.Index, new { id = model.BudgetId });
        }

        #region private

        /// <summary>
        ///     Get list of users which don't have permissions to the budget yet.
        /// </summary>
        /// <param name="users">List of Guids of users with permission</param>
        /// <returns></returns>
        private async Task<List<SelectListItem>> GetUsers(ICollection<Guid> users)
        {
            // Id of current user
            var currentUserId = await this.CurrentProfileId();

            var userProfiles = this._budgetAccessRightService.GetUserProfiles(users, currentUserId);

            return
                await
                    userProfiles
                        .Select(user => new SelectListItem { Value = user.Guid.ToString(), Text = user.FirstName })
                        .ToListAsync();
        }

        /// <summary>
        /// Prepares string with ApplicationId, budget id, assigned user id and selected permission.
        /// </summary>
        /// <param name="model">budget access right model with set id, email and permission</param>
        /// <returns>string with ApplicationId, budget id, assigned user id and selected permission</returns>
        private string getConfirmationString(CreateBudgetAccessRightModel model)
        {
            return String.Join(
                Environment.NewLine,
                new[] {
                    ConfigurationManager.AppSettings["ApplicationId"],
                    model.BudgetId.ToString(),
                    model.AssignedUserEmail,
                    model.Permission.ToString(),
                });
        }

        #endregion
    }
}