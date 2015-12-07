using System;
using ExpenseManager.BusinessLogic.WalletServices;
using ExpenseManager.Entity.Currencies;
using ExpenseManager.Entity.Providers.Factory;
using ExpenseManager.Entity.Wallets;
using NUnit.Framework;

namespace ExpenseManager.BusinessLogic.Test.WalletTests
{
    [TestFixture]
    internal class WalletServiceTest
    {
        [Test]
        [ExpectedException(typeof (ServiceValidationException))]
        public void ValidateWallet_EmptyName_ThrowException()
        {
            var wallet = new Wallet
            {
                Name = string.Empty,
                Currency = new Currency()
            };

            var walletService = new WalletService(ProvidersFactory.GetNewWalletsProviders());
            walletService.Validate(wallet);
        }

        [Test]
        [ExpectedException(typeof (ServiceValidationException))]
        public void ValidateWallet_NullCurrency_ThrowException()
        {
            var wallet = new Wallet
            {
                Name = "Test name",
                Currency = null
            };

            var walletService = new WalletService(ProvidersFactory.GetNewWalletsProviders());
            walletService.Validate(wallet);
        }

        [Test]
        [TestCase(null)]
        [ExpectedException(typeof (ArgumentNullException))]
        public void ValidateWallet_NullWallet_ThrowException(Wallet wallet)
        {
            var walletService = new WalletService(ProvidersFactory.GetNewWalletsProviders());
            walletService.Validate(wallet);
        }

        [Test]
        public void ValidateWallet_ValidWallet_ReturnTrue()
        {
            var wallet = new Wallet
            {
                Name = "Test name",
                Currency = new Currency
                {
                    Name = "Default currency",
                    Code = "DC",
                    Symbol = "C"
                }
            };

            var walletService = new WalletService(ProvidersFactory.GetNewWalletsProviders());
            Assert.DoesNotThrow(() => walletService.Validate(wallet));
        }
    }
}