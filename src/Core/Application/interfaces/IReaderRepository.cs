using Domain.Models;
using Application.Filters;

namespace Application.Interfaces
{
    public interface IReaderRepository : IRepository<Reader>
    {
        Task<IEnumerable<Domain.Models.Reader>> Filter(ReaderFilter? filter = null, CancellationToken ct = default);
    }
}
