using System.Threading;
using System.Threading.Tasks;

namespace Application.Books.Services
{
    public interface IBookDeletionService
    {
        Task EnsureCanDeleteAsync(string bookId, CancellationToken ct = default);
    }
}
