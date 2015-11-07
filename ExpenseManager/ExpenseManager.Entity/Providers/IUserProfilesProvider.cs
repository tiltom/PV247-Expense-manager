using ExpenseManager.Entity.Providers.infrastructure;
using ExpenseManager.Entity.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseManager.Entity.Providers
{
    public interface IUserProfilesProvider : IAddOrUpdateDeleteEntityProvider<UserProfile>
    {
        IQueryable<UserProfile> UserProfiles { get; }
    }
}
