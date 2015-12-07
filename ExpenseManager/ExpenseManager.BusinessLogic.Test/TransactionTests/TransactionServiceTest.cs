using System;
using ExpenseManager.BusinessLogic.TransactionServices;
using ExpenseManager.BusinessLogic.TransactionServices.Models;
using ExpenseManager.Entity.Enums;
using ExpenseManager.Entity.Providers.Factory;
using NUnit.Framework;

namespace ExpenseManager.BusinessLogic.Test.TransactionTests
{
    [TestFixture]
    internal class TransactionServiceTest
    {
        private readonly TransactionService _transactionService =
            new TransactionService(ProvidersFactory.GetNewBudgetsProviders(),
                ProvidersFactory.GetNewTransactionsProviders(), ProvidersFactory.GetNewWalletsProviders());

        private TransactionServiceModel _model;
        private TransactionServiceModel _repeatableModel;

        [TestFixtureSetUp]
        public void Init()
        {
            this._model = new TransactionServiceModel
            {
                Amount = 50,
                Date = DateTime.Now,
                WalletId = Guid.NewGuid(),
                CurrencyId = Guid.NewGuid(),
                CategoryId = Guid.NewGuid(),
                IsRepeatable = false
            };

            this._repeatableModel = new TransactionServiceModel
            {
                Amount = 50,
                Date = DateTime.Now,
                WalletId = Guid.NewGuid(),
                CurrencyId = Guid.NewGuid(),
                CategoryId = Guid.NewGuid(),
                IsRepeatable = true,
                NextRepeat = 2,
                FrequencyType = FrequencyType.Week,
                LastOccurrence = DateTime.Now.AddDays(30)
            };
        }

        [Test]
        [ExpectedException(typeof (ServiceValidationException))]
        public void Create_Empty_Transaction()
        {
            this._transactionService.Validate(new TransactionServiceModel());
        }

        [Test]
        public void Validate_Good_RepeatableTransaction()
        {
            this._transactionService.Validate(this._repeatableModel);
        }

        [Test]
        public void Validate_Good_Transaction()
        {
            this._transactionService.Validate(this._model);
        }

        [Test]
        [ExpectedException(typeof (ServiceValidationException))]
        public void Validate_NegativeNextRepeat_RepeatableTransaction()
        {
            this._repeatableModel.NextRepeat = -1;
            this._transactionService.Validate(this._repeatableModel);
        }

        [Test]
        public void Validate_NoBudgetId_Transaction()
        {
            this._model.BudgetId = Guid.Empty;
            this._transactionService.Validate(this._model);
        }

        [Test]
        [ExpectedException(typeof (ServiceValidationException))]
        public void Validate_NoCategoryId_Transaction()
        {
            this._model.CategoryId = Guid.Empty;
            this._transactionService.Validate(this._model);
        }

        [Test]
        [ExpectedException(typeof (ServiceValidationException))]
        public void Validate_NoCurrencyId_Transaction()
        {
            this._model.CurrencyId = Guid.Empty;
            this._transactionService.Validate(this._model);
        }

        [Test]
        [ExpectedException(typeof (ServiceValidationException))]
        public void Validate_NoWalletId_Transaction()
        {
            this._model.WalletId = Guid.Empty;
            this._transactionService.Validate(this._model);
        }

        [Test]
        [TestCase(null)]
        [ExpectedException(typeof (ServiceValidationException))]
        public void Validate_NullLastOccurrence_RepeatableTransaction(DateTime? lastOccurrence)
        {
            this._repeatableModel.LastOccurrence = lastOccurrence;
            this._transactionService.Validate(this._repeatableModel);
        }

        [Test]
        [TestCase(null)]
        [ExpectedException(typeof (ServiceValidationException))]
        public void Validate_NullNextRepeat_RepeatableTransaction(int? nextRepeat)
        {
            this._repeatableModel.NextRepeat = nextRepeat;
            this._transactionService.Validate(this._repeatableModel);
        }

        [Test]
        [ExpectedException(typeof (ServiceValidationException))]
        public void Validate_SameLastOccurenceAsDate_RepeatableTransaction()
        {
            this._repeatableModel.LastOccurrence = DateTime.Now;
            this._transactionService.Validate(this._repeatableModel);
        }

        [Test]
        [ExpectedException(typeof (ServiceValidationException))]
        public void Validate_WrongLastOccurence_RepeatableTransaction()
        {
            this._repeatableModel.LastOccurrence = DateTime.Now.AddDays(-3);
            this._transactionService.Validate(this._repeatableModel);
        }

        [Test]
        [ExpectedException(typeof (ServiceValidationException))]
        public void Validate_ZeroNextRepeat_RepeatableTransaction()
        {
            this._repeatableModel.NextRepeat = 0;
            this._transactionService.Validate(this._repeatableModel);
        }

        [Test]
        [TestCase(null)]
        [ExpectedException(typeof (ArgumentNullException))]
        public void ValidateTransaction_Null(TransactionServiceModel serviceModel)
        {
            this._transactionService.Validate(serviceModel);
        }
    }
}