namespace ItPlanet.Database;

public interface IRepository<TEntity, TId>
{
    Task<TEntity?> GetByIdAsync(TId key);
}