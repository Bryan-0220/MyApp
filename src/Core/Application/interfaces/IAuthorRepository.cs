using Domain.Models;

namespace Application.Interfaces
{
    public interface IAuthorRepository : IRepository<Author>
    {
        Task<IEnumerable<Author>> FilterAsync(string? name = null, string? nationality = null, System.Collections.Generic.IEnumerable<string>? genres = null, CancellationToken ct = default);
    }
}
