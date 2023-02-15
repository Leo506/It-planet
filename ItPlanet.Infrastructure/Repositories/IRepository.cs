namespace ItPlanet.Infrastructure.Repositories;

public interface IRepository<TModel, in TId>
{
    Task<TModel?> GetAsync(TId id);

    Task<List<TModel>> GetAllAsync();

    Task<TModel> CreateAsync(TModel model);

    Task CreateRangeAsync(IEnumerable<TModel> models);

    Task<TModel> UpdateAsync(TModel model);

    Task UpdateRangeAsync(IEnumerable<TModel> models);

    Task DeleteAsync(TModel model);

    Task DeleteRangeAsync(IEnumerable<TModel> models);
}