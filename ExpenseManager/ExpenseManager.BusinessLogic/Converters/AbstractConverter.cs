using System.Collections.Generic;
using System.Linq;
using ExpenseManager.Entity;

namespace ExpenseManager.BusinessLogic.Converters
{
    internal abstract class AbstractConverter<TEntity, TDto>
        where TEntity : class, IEntity
        where TDto : class
    {
        public abstract TEntity EntityFromDto(TDto dto);
        public abstract TDto DtoFromEntity(TEntity entity);

        public ICollection<TEntity> EntityFromDto(ICollection<TDto> dtos)
        {
            return dtos.Select(dto => this.EntityFromDto(dto)).ToList();
        }

        public ICollection<TDto> DtoFromEntity(ICollection<TEntity> entities)
        {
            return entities.Select(entity => this.DtoFromEntity(entity)).ToList();
        }
    }
}