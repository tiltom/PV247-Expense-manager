using System;
using ExpenseManager.Entity;

namespace ExpenseManager.BusinessLogic
{
    public class CommonService
    {
        /// <summary>
        ///     Returns permission profile by string identification (if no matching found Permission.Read is returned)
        /// </summary>
        /// <param name="permission">permission string</param>
        /// <returns>instance of Permission enum</returns>
        public PermissionEnum ConvertPermissionStringToEnum(string permission)
        {
            var defaultPermission = PermissionEnum.Read;
            Enum.TryParse(permission, out defaultPermission);
            return defaultPermission;
        }
    }
}