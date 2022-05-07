using System.Linq.Expressions;
using System.Reflection;

namespace Entr.Data.EntityFramework.Mapping
{
    public static class MappingHelpers
    {
        public static Expression<Func<TEntity, TResult>> GetMember<TEntity, TResult>(string memberName)
        {
            var parameter = Expression.Parameter(typeof(TEntity), "p");
        
            var member = Expression.MakeMemberAccess(
                parameter, 
                typeof(TEntity).GetMember(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Single());

            return Expression.Lambda<Func<TEntity, TResult>>(member, parameter);
        }
    }
}
