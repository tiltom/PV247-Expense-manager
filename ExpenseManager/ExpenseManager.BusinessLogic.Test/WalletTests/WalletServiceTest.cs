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
        public void ValidateWallet_EmptyName_ReturnFalse()
        {
            var wallet = new Wallet
            {
                Name = string.Empty,
                Currency = new Currency()
            };

            var walletService = new WalletService(ProvidersFactory.GetNewWalletsProviders());
            Assert.IsFalse(walletService.ValidateWallet(wallet));
        }

        [Test]
        public void ValidateWallet_NullCurrency_ReturnFalse()
        {
            var wallet = new Wallet
            {
                Name = "Test name",
                Currency = null
            };

            var walletService = new WalletService(ProvidersFactory.GetNewWalletsProviders());
            Assert.IsFalse(walletService.ValidateWallet(wallet));
        }

        [Test]
        public void ValidateWallet_NullOwner_ReturnFalse()
        {
            var wallet = new Wallet
            {
                Name = "Test name",
                Currency = new Currency()
            };

            var walletService = new WalletService(ProvidersFactory.GetNewWalletsProviders());
            Assert.IsFalse(walletService.ValidateWallet(wallet));
        }

        [Test]
        [TestCase(null)]
        public void ValidateWallet_NullWallet_ReturnFalse(Wallet wallet)
        {
            var walletService = new WalletService(ProvidersFactory.GetNewWalletsProviders());
            Assert.IsFalse(walletService.ValidateWallet(wallet));
        }

        [Test]
        public void ValidateWallet_ValidWallet_ReturnTrue()
        {
            var wallet = new Wallet
            {
                Name = "Test name",
                Currency = new Currency()
            };

            var walletService = new WalletService(ProvidersFactory.GetNewWalletsProviders());
            Assert.IsTrue(walletService.ValidateWallet(wallet));
        }
    }
}