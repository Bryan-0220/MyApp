using Domain.Models;

namespace Application.Interfaces
{
    public interface IReaderRepository : IRepository<Reader>
    {
        Task<IEnumerable<Reader>> FilterAsync(Application.Filters.ReaderFilter? filter = null, CancellationToken ct = default);
    }
}
