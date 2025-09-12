using Domain.Models;
using Application.Filters;

namespace Application.Interfaces
{
    public interface IReaderRepository : IRepository<Reader>
    {
        Task<IEnumerable<Reader>> FilterAsync(ReaderFilter? filter = null, CancellationToken ct = default);
    }
}
