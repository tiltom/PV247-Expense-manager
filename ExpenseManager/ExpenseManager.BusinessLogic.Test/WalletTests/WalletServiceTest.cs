using System;
using System.Collections.Generic;
using System.Linq;
using ExpenseManager.BusinessLogic.WalletServices;
using ExpenseManager.Entity.Currencies;
using ExpenseManager.Entity.Providers;
using Moq;
using NUnit.Framework;

namespace ExpenseManager.BusinessLogic.Test.WalletTests
{
    [TestFixture]
    public class WalletServiceTest
    {
        [SetUp]
        public void Init()
        {
            var walletsProvider = new Mock<IWalletsProvider>();
        }

        private WalletService _walletService;

        [Test]
        public void GetCurrencies_ReturnListOfCurrencies()
        {
            var currency = new List<Currency>
            {
                new Currency
                {
                    Guid = Guid.Empty,
                    Name = "Test currency",
                    Symbol = "Tc"
                }
            }.AsQueryable();

            var walletsMock = new Mock<IWalletsProvider>();
            walletsMock.Setup(m => m.Currencies).Returns(currency);

            this._walletService = new WalletService(walletsMock.Object);

            Assert.That(this._walletService.GetCurrencies(), Is.Not.Empty);
        }
    }
}