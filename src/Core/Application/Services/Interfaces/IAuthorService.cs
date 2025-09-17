using Domain.Models;
using Domain.Results;

namespace Application.Authors.Services
{
    public interface IAuthorService
    {
        Task EnsureCanCreate(string name, CancellationToken ct = default);
        Task EnsureCanDelete(string authorId, CancellationToken ct = default);
        Task<Result<Author>> DeleteAuthor(string authorId, CancellationToken ct = default);
        public async Task<Author> CreateAuthor(AuthorData input, CancellationToken ct);
    }
}
