using System.Threading;
using System.Threading.Tasks;

namespace Application.Authors.Services
{
    public interface IAuthorDeletionService
    {
        Task EnsureCanDeleteAsync(string authorId, CancellationToken ct = default);
    }
}
