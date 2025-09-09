using Domain.Models;
using Application.Filters;

namespace Application.Interfaces
{
    public interface IAuthorRepository : IRepository<Author>
    {
        Task<IEnumerable<Author>> FilterAsync(AuthorFilter? filter = null, CancellationToken ct = default);
    }
}
