using Domain.Models;
using Application.Filters;

namespace Application.Interfaces
{
    public interface IAuthorRepository : IRepository<Author>
    {
        // Task<IEnumerable<Author>> FilterAsync(string? name = null, string? nationality = null, System.Collections.Generic.IEnumerable<string>? genres = null, CancellationToken ct = default);
        Task<IEnumerable<Author>> FilterAsync(AuthorFilter? filter = null, CancellationToken ct = default);
    }
}
