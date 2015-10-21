using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using ExpenseManager.Entity;
using ExpenseManager.Web.Models.Transaction;
using ExpenseManager.Web.Models.User;
using Microsoft.AspNet.Identity;

namespace ExpenseManager.Web.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        private async Task<Transaction> ConvertModelToEntity(TransactionModel model, Transaction entity)
        {
            entity.Amount = model.Amount;
            entity.Date = model.Date;
            entity.Description = model.Description;
            entity.Wallet = await this._db.Wallets.FindAsync(model.WalletId);
            //budget
            entity.Currency = await this._db.Currencies.FindAsync(new Guid(model.CurrencyId));
            entity.Category = await this._db.Categories.FindAsync(new Guid(model.CategoryId));
            return entity;
        }

        private TransactionModel ConvertEntityToModel(Transaction entity)
        {
            var repeatableTransaction =
                this._db.RepeatableTransactions.FirstOrDefault(a => a.FirstTransaction.Guid == entity.Guid);
            var transactionModel = new TransactionModel
            {
                Id = entity.Guid,
                Amount = entity.Amount,
                Date = entity.Date,
                Description = entity.Description,
                WalletId = entity.Wallet.Guid,
                CurrencyId = entity.Currency.Guid.ToString(),
                CurrencySymbol = entity.Currency.Symbol,
                CategoryId = entity.Category.Guid.ToString(),
                CategoryName = entity.Category.Name
            };

            if (repeatableTransaction != null)
            {
                transactionModel.IsRepeatable = true;
                transactionModel.Frequency = repeatableTransaction.Frequency;
                transactionModel.LastOccurence = repeatableTransaction.LastOccurence;
            }

            return transactionModel;
        }

        // GET: Transactions
        public async Task<ActionResult> Index()
        {
            var id = HttpContext.User.Identity.GetUserId();
            var list = await this._db.Transactions.Where(user => user.Wallet.Owner.Id == id).ToListAsync();
            return this.View(list.Select(this.ConvertEntityToModel));
        }

        // GET: Transactions/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var transaction = await this._db.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return this.HttpNotFound();
            }
            var model = this.ConvertEntityToModel(transaction);
            model.Currencies = await this.GetCurrencies();
            model.Categories = await this.GetCategories();
            return this.View(model);
        }

        // GET: Transactions/Create
        public async Task<ActionResult> Create()
        {
            return
                this.View(new TransactionModel
                {
                    WalletId = await this.GetUserWalletId(),
                    Categories = await this.GetCategories(),
                    Currencies = await this.GetCurrencies()
                });
        }

        private async Task<List<SelectListItem>> GetCurrencies()
        {
            return
                await
                    this._db.Currencies.Select(
                        currency => new SelectListItem {Value = currency.Guid.ToString(), Text = currency.Name})
                        .ToListAsync();
        }

        private async Task<List<SelectListItem>> GetCategories()
        {
            return
                await
                    this._db.Categories.Select(
                        category => new SelectListItem {Value = category.Guid.ToString(), Text = category.Name})
                        .ToListAsync();
        }

        private async Task<Guid> GetUserWalletId()
        {
            var id = HttpContext.User.Identity.GetUserId();
            var walletEntity = await this._db.Wallets.Where(wallet => wallet.Owner.Id == id).FirstOrDefaultAsync();
            return walletEntity.Guid;
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(TransactionModel transaction)
        {
            if (ModelState.IsValid)
            {
                var transactionEntity =
                    await this.ConvertModelToEntity(transaction, new Transaction {Guid = Guid.NewGuid()});
                this._db.Transactions.Add(transactionEntity);
                if (transaction.IsRepeatable)
                {
                    var repeatableTransaction = new RepeatableTransaction
                    {
                        FirstTransaction = transactionEntity,
                        Frequency = transaction.Frequency,
                        LastOccurence = transaction.LastOccurence.GetValueOrDefault(),
                        Guid = Guid.NewGuid()
                    };
                    this._db.RepeatableTransactions.Add(repeatableTransaction);
                }
                await this._db.SaveChangesAsync();
                return this.RedirectToAction("Index");
            }
            transaction.Currencies = await this.GetCurrencies();
            transaction.Categories = await this.GetCategories();
            return this.View(transaction);
        }

        // GET: Transactions/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var transaction = await this._db.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return this.HttpNotFound();
            }
            var model = this.ConvertEntityToModel(transaction);
            model.Currencies = await this.GetCurrencies();
            model.Categories = await this.GetCategories();
            return this.View(model);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(TransactionModel transaction)
        {
            if (ModelState.IsValid)
            {
                var transactionEntity = await this._db.Transactions.FindAsync(transaction.Id);
                await this.ConvertModelToEntity(transaction, transactionEntity);
                var repeatableTransaction =
                    await
                        this._db.RepeatableTransactions.FirstOrDefaultAsync(
                            a => a.FirstTransaction.Guid == transaction.Id);
                if (transaction.IsRepeatable)
                {
                    if (repeatableTransaction != null)
                    {
                        repeatableTransaction.Frequency = transaction.Frequency;
                        repeatableTransaction.LastOccurence = transaction.LastOccurence.GetValueOrDefault();
                        this._db.RepeatableTransactions.Attach(repeatableTransaction);
                        this._db.Entry(repeatableTransaction).State = EntityState.Modified;
                    }
                    else
                    {
                        repeatableTransaction = new RepeatableTransaction
                        {
                            FirstTransaction = transactionEntity,
                            Frequency = transaction.Frequency,
                            LastOccurence = transaction.LastOccurence.GetValueOrDefault(),
                            Guid = Guid.NewGuid()
                        };
                        this._db.RepeatableTransactions.Add(repeatableTransaction);
                    }
                }
                else
                {
                    if (repeatableTransaction != null)
                    {
                        this._db.RepeatableTransactions.Remove(repeatableTransaction);
                    }
                }
                await this._db.SaveChangesAsync();
                return this.RedirectToAction("Index");
            }
            transaction.Currencies = await this.GetCurrencies();
            transaction.Categories = await this.GetCategories();
            return this.View(transaction);
        }

        // GET: Transactions/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var transaction = await this._db.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return this.HttpNotFound();
            }
            var model = this.ConvertEntityToModel(transaction);
            model.Currencies = await this.GetCurrencies();
            model.Categories = await this.GetCategories();
            return this.View(model);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            var transaction = await this._db.Transactions.FindAsync(id);
            var repeatableTransaction =
                await this._db.RepeatableTransactions.FirstOrDefaultAsync(a => a.FirstTransaction.Guid == id);
            if (repeatableTransaction != null)
            {
                this._db.RepeatableTransactions.Remove(repeatableTransaction);
            }
            this._db.Transactions.Remove(transaction);
            await this._db.SaveChangesAsync();
            return this.RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}