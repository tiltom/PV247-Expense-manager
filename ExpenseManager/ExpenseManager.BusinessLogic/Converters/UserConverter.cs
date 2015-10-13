using ExpenseManager.BusinessLogic.DataTransferObjects;
using ExpenseManager.Entity;

namespace ExpenseManager.BusinessLogic.Converters
{
    internal sealed class UserConverter : AbstractConverter<User, UserDTO>
    {
        public static UserConverter Converter = new UserConverter();

        private UserConverter()
        {
        }

        public override User EntityFromDto(UserDTO dto)
        {
            return new User
            {
                Id = dto.Id,
                Email = dto.Email,
                Password = dto.Password, // will be removed
                CreateTime = dto.CreateTime,
                UserName = dto.UserName
                // TODO converters has to be added
                // eg. AccessRightsToBudgets = BudgetAccessRightConverter.EntityFromDto(dto.AccessRightsToBudgets)
            };
        }

        public override UserDTO DtoFromEntity(User entity)
        {
            return new UserDTO
            {
                Id = entity.Id,
                Email = entity.Email,
                Password = entity.Password,
                CreateTime = entity.CreateTime,
                UserName = entity.UserName
                // TODO converters has to be added
                // eg. AccessRightsToBudgets = BudgetAccessRightConverter.DtoFromEntity(entity.AccessRightsToBudgets)
            };
        }
    }
}