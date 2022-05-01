using System;

namespace Entr.Domain
{
    public static class EntityIdSetter
    {
        public static void SetId<TEntity, TId>(TEntity entity, TId id)
            where TEntity : Entity<TId>
            where TId : struct
        {
            entity.Id = id;
        }
    }
}
