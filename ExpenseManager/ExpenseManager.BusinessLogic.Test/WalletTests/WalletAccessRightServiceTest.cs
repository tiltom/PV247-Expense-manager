using ExpenseManager.BusinessLogic.WalletServices;
using ExpenseManager.Entity;
using ExpenseManager.Entity.Currencies;
using ExpenseManager.Entity.Providers.Factory;
using ExpenseManager.Entity.Users;
using ExpenseManager.Entity.Wallets;
using NUnit.Framework;

namespace ExpenseManager.BusinessLogic.Test.WalletTests
{
    [TestFixture]
    internal class WalletAccessRightServiceTest
    {
        [Test]
        public void ValidateWalletAccessRight_NullUserProfile_ReturnFalse()
        {
            var walletAccessRight = new WalletAccessRight
            {
                Wallet = new Wallet(),
                Permission = PermissionEnum.Read,
                UserProfile = null
            };

            var walletAccessRightService = new WalletAccessRightService(ProvidersFactory.GetNewWalletsProviders(),
                new CommonService());
            Assert.IsFalse(walletAccessRightService.ValidateWalletAccessRight(walletAccessRight));
        }

        [Test]
        public void ValidateWalletAccessRight_NullWallet_ReturnFalse()
        {
            var walletAccessRight = new WalletAccessRight
            {
                Wallet = null,
                Permission = PermissionEnum.Read,
                UserProfile = new UserProfile()
            };

            var walletAccessRightService = new WalletAccessRightService(ProvidersFactory.GetNewWalletsProviders(),
                new CommonService());
            Assert.IsFalse(walletAccessRightService.ValidateWalletAccessRight(walletAccessRight));
        }

        [Test]
        [TestCase(null)]
        public void ValidateWalletAccessRight_NullWalletAccessRight_ReturnFalse(
            WalletAccessRight walletAccessRight)
        {
            var walletAccessRightService = new WalletAccessRightService(ProvidersFactory.GetNewWalletsProviders(),
                new CommonService());
            Assert.IsFalse(walletAccessRightService.ValidateWalletAccessRight(walletAccessRight));
        }

        [Test]
        public void ValidateWalletAccessRight_ValidWallet_ReturnTrue()
        {
            var walletAccessRight = new WalletAccessRight
            {
                Wallet = new Wallet
                {
                    Name = "Default Wallet",
                    Currency = new Currency
                    {
                        Name = "Default currency",
                        Code = "DC",
                        Symbol = "C"
                    }
                },
                Permission = PermissionEnum.Read,
                UserProfile = new UserProfile()
            };

            var walletAccessRightService = new WalletAccessRightService(ProvidersFactory.GetNewWalletsProviders(),
                new CommonService());
            Assert.IsTrue(walletAccessRightService.ValidateWalletAccessRight(walletAccessRight));
        }
    }
}