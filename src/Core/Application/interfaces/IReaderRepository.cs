using Domain.Models;

namespace Application.Interfaces
{
    public interface IReaderRepository : IRepository<Reader>
    {
        Task<IEnumerable<Reader>> FilterAsync(Filters.ReaderFilter? filter = null, CancellationToken ct = default);
    }
}
