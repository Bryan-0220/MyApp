using Domain.Models;
using Application.Filters;

namespace Application.Interfaces
{
    public interface IBookRepository : IRepository<Book>
    {
        Task<IEnumerable<Book>> FilterAsync(BookFilter? filter = null, CancellationToken ct = default);
        Task<bool> TryChangeCopiesAsync(string bookId, int delta, CancellationToken ct = default);
    }
}
