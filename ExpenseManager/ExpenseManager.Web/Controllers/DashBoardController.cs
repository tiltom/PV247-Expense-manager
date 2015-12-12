using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using ExpenseManager.BusinessLogic;
using ExpenseManager.BusinessLogic.DashboardServices;
using ExpenseManager.BusinessLogic.DashboardServices.Models;
using ExpenseManager.BusinessLogic.TransactionServices;
using ExpenseManager.Entity.Providers.Factory;
using ExpenseManager.Web.Constants;
using ExpenseManager.Web.Models.HomePage;

namespace ExpenseManager.Web.Controllers
{
    [RequireHttps]
    public class DashBoardController : AbstractController
    {
        private readonly DashBoardService _dashBoardService =
            new DashBoardService(ProvidersFactory.GetNewTransactionsProviders(), new ColorGeneratorService());

        private readonly TransactionService _transactionService =
            new TransactionService(ProvidersFactory.GetNewBudgetsProviders(),
                ProvidersFactory.GetNewTransactionsProviders(), ProvidersFactory.GetNewWalletsProviders());

        /// <summary>
        ///     Will display empty page with initialized filter
        /// </summary>
        /// <returns>Initialized filter</returns>
        public async Task<ViewResult> Index()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return this.View(SharedConstant.Index, await this.ProcessFilter(new FilterDataModel()));
            }
            return this.View();
        }


        /// <summary>
        ///     Will display empty page with initialized filter
        /// </summary>
        /// <returns>Initialized filter</returns>
        [HttpGet]
        [Authorize]
        public async Task<ViewResult> IndexWithFilter(FilterDataModel filter)
        {
            return this.View(SharedConstant.Index, await this.ProcessFilter(filter));
        }

        private async Task<DashBoardModel> ProcessFilter(FilterDataModel filter)
        {
            var mappedFilter = Mapper.Map<FilterDataServiceModel>(filter);
            var data =
                await
                    this._dashBoardService.GenerateDataForFilter(
                        mappedFilter,
                        await this.CurrentProfileId()
                        );
            var result = Mapper.Map<DashBoardModel>(data);
            result.Filter = await this.InitFilter(filter);
            return result;
        }

        /// <summary>
        ///     Action for adding new repeatable transactions
        /// </summary>
        /// <returns>View with model</returns>
        // GET: Home/UpdateRepeatable
        public async Task<ActionResult> UpdateRepeatable()
        {
            return await this._transactionService.UpdateRepeatableTransactions();
        }


        private async Task<FilterDataModel> InitFilter(FilterDataModel filter)
        {
            var userId = await this.CurrentProfileId();
            filter.BudgetsSelectList = await this._transactionService.GetReadableBudgetsSelection(userId);
            filter.WalletsSelectList = await this._transactionService.GetAllReadableWalletsSelection(userId);
            filter.CategoriesSelectList = await this._transactionService.GetAllCategoriesSelection();
            return filter;
        }
    }
}