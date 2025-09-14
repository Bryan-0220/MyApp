using System.Threading;
using System.Threading.Tasks;

namespace Application.Readers.Services
{
    public interface IReaderDeletionService
    {
        Task EnsureCanDeleteAsync(string readerId, CancellationToken ct = default);
    }
}
