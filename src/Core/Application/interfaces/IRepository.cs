using System.Linq.Expressions;

namespace Application.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(string id, CancellationToken ct = default);
        Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default);
        Task<IEnumerable<T>> ListAsync(CancellationToken ct = default);
        Task<IEnumerable<T>> FilterAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default);
        Task<T> CreateAsync(T entity, CancellationToken ct = default);
        Task<bool> UpdateAsync(string id, T entity, CancellationToken ct = default);
        Task UpdateAsync(T entity, CancellationToken ct = default);
        Task<bool> DeleteAsync(string id, CancellationToken ct = default);
        Task<long> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default);
    }
}
