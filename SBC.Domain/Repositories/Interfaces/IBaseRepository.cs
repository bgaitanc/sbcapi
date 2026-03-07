namespace SBC.Domain.Repositories.Interfaces;

public interface IBaseRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);
    Task<T?> GetByIdNoTrackingAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> GetAllNoTrackingAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}
