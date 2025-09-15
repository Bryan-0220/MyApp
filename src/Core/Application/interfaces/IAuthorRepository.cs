using Domain.Models;
using Application.Filters;

namespace Application.Interfaces
{
    public interface IAuthorRepository : IRepository<Author>
    {
        Task<IEnumerable<Author>> Filter(AuthorFilter? filter = null, CancellationToken ct = default);
    }
}
