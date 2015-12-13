namespace ExpenseManager.Web.Constants.LoginConstants
{
    public class LoginConstant
    {
        public const string Lockout = "Lockout";
        public const string ConfirmEmail = "ConfirmEmail";
        public const string Account = "Account";
        public const string Login = "Login";
        public const string Manage = "Manage";
        public const string ExternalLoginFailure = "ExternalLoginFailure";
        public const string ExternalLoginCallback = "ExternalLoginCallback";
        public const string ExternalLoginConfirmation = "ExternalLoginConfirmation";
        public const string Facebook = "Facebook";
        public const string FacebookAccessToken = "FacebookAccessToken";
        public const string ManageLogins = "ManageLogins";
        public const string LinkLoginCallback = "LinkLoginCallback";
        public const string ExternalLogin = "ExternalLogin";

        // Used for XSRF protection when adding external logins
        public const string XsrfKey = "XsrfId";
    }
}