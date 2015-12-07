using System;
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
        [ExpectedException(typeof (ServiceValidationException))]
        public void ValidateWalletAccessRight_NullUserProfile_ThrowException()
        {
            var walletAccessRight = new WalletAccessRight
            {
                Wallet = new Wallet(),
                Permission = PermissionEnum.Read,
                UserProfile = null
            };

            var walletAccessRightService = new WalletAccessRightService(ProvidersFactory.GetNewWalletsProviders(),
                new CommonService());
            walletAccessRightService.Validate(walletAccessRight);
        }

        [Test]
        [ExpectedException(typeof (ServiceValidationException))]
        public void ValidateWalletAccessRight_NullWallet_ThrowException()
        {
            var walletAccessRight = new WalletAccessRight
            {
                Wallet = null,
                Permission = PermissionEnum.Read,
                UserProfile = new UserProfile()
            };

            var walletAccessRightService = new WalletAccessRightService(ProvidersFactory.GetNewWalletsProviders(),
                new CommonService());
            walletAccessRightService.Validate(walletAccessRight);
        }

        [Test]
        [TestCase(null)]
        [ExpectedException(typeof (ArgumentNullException))]
        public void ValidateWalletAccessRight_NullWalletAccessRight_ThrowException(
            WalletAccessRight walletAccessRight)
        {
            var walletAccessRightService = new WalletAccessRightService(ProvidersFactory.GetNewWalletsProviders(),
                new CommonService());
            walletAccessRightService.Validate(walletAccessRight);
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
            Assert.DoesNotThrow(() => walletAccessRightService.Validate(walletAccessRight));
        }
    }
}