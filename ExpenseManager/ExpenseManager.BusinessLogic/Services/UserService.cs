using ExpenseManager.BusinessLogic.Converters;
using ExpenseManager.BusinessLogic.DataAccessObjects;
using ExpenseManager.BusinessLogic.DataTransferObjects;

namespace ExpenseManager.BusinessLogic.Services
{
    public class UserService
    {
        private readonly UserDAO _userDao;

        public UserService()
        {
            this._userDao = new UserDAO();
        }

        public void CreateUser(UserDTO user)
        {
            this._userDao.Create(UserConverter.Converter.EntityFromDto(user));
            this._userDao.SaveChanges();
        }

        public void DeleteUser(UserDTO user)
        {
            this._userDao.Delete(UserConverter.Converter.EntityFromDto(user));
            this._userDao.SaveChanges();
        }
    }
}