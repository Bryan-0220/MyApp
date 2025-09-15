using System.Linq.Expressions;

namespace Application.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetById(string id, CancellationToken ct = default);
        Task<IEnumerable<T>> GetAll(CancellationToken rt = default);
        Task<IEnumerable<T>> List(CancellationToken ct = default);
        Task<T> Create(T entity, CancellationToken ct = default);
        Task<bool> Update(string id, T entity, CancellationToken ct = default);
        Task Update(T entity, CancellationToken ct = default);
        Task<bool> Delete(string id, CancellationToken ct = default);
        Task<long> Count(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default);
    }
}
