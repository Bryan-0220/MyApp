using Domain.Models;
using Application.Filters;

namespace Application.Interfaces
{
    public interface IReaderRepository : IRepository<Reader>
    {
        Task<IEnumerable<Reader>> Filter(ReaderFilter? filter = null, CancellationToken ct = default);
    }
}
