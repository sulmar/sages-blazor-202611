using Domain.Models;

namespace Domain.Abstractions;

public interface IEntityRepository<T>
    where T : BaseEntity
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<Page<T>> GetPageAsync(int skip, int take);
    Task<T?> GetByIdAsync(int id);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}
