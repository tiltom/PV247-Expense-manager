using System.Linq;
using ExpenseManager.Entity.Providers.infrastructure;
using ExpenseManager.Entity.Users;

namespace ExpenseManager.Entity.Providers
{
    public interface IUserProfilesProvider : IAddOrUpdateDeleteEntityProvider<UserProfile>
    {
        IQueryable<UserProfile> UserProfiles { get; }
    }
}