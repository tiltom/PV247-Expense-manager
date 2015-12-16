namespace ExpenseManager.Web.Constants.UserConstants
{
    public sealed class UserConstant
    {
        public const int NameMaximumLength = 100;
        public const int NameMinimumLength = 2;
        public const int PasswordMinimumLength = 6;
        public const string EditRoleInclude = "Name,Id";
        public const string DescriptiveWalletName = "{0} {1}'s {2}";

        public static string FormatWalletName(string userName, string surname, string walletName)
        {
            return string.Format(DescriptiveWalletName, userName, surname, walletName);
        }
    }
}