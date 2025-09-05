using Domain.Models;
using Application.Filters;

namespace Application.Interfaces
{
    public interface IBookRepository : IRepository<Book>
    {
        Task<bool> TryChangeCopiesAsync(string bookId, int delta, CancellationToken ct = default);

        Task<IEnumerable<Book>> FilterAsync(BookFilter? filter = null, CancellationToken ct = default);
    }
}
