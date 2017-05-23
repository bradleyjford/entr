using System.Linq;

namespace Entr.Data
{
    public interface IQueryFilter<T>
    {
        IQueryable<T> Apply(IQueryable<T> patients);
    }
}