using Domain.Models;
using Application.Filters;

namespace Application.Interfaces
{
    public interface IBookRepository : IRepository<Book>
    {
        Task<IEnumerable<Book>> FilterAsync(BookFilter? filter = null, CancellationToken ct = default);
    }
}
