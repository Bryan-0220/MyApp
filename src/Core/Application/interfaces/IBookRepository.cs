using Domain.Models;
using Application.Filters;

namespace Application.Interfaces
{
    public interface IBookRepository : IRepository<Book>
    {
        Task<IEnumerable<Book>> Filter(BookFilter? filter = null, CancellationToken ct = default);
        Task<bool> TryChangeCopies(string bookId, int delta, CancellationToken ct = default);
    }
}
