using ItPlanet.Models;

namespace ItPlanet.Database;

public interface IRepository<TEntity, TId>
{
    Task<TEntity?> GetByIdAsync(TId key);
    Task<IEnumerable<TEntity>> GetByPredicate(Func<TEntity, bool> predicate);
}