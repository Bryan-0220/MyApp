using Domain.Models;
using Domain.Results;
using UpdateAuthor;

namespace Application.Authors.Services
{
    public interface IAuthorService
    {
        Task EnsureCanCreate(string name, CancellationToken ct = default);
        Task EnsureCanDelete(string authorId, CancellationToken ct = default);
        Task<Result<Author>> DeleteAuthor(string authorId, CancellationToken ct = default);
        Task<Author> CreateAuthor(AuthorData input, CancellationToken ct = default);
        Task<Author> UpdateAuthor(UpdateAuthorCommandInput input, CancellationToken ct = default);
    }
}
