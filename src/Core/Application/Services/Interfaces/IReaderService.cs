using System.Threading;
using System.Threading.Tasks;

namespace Application.Readers.Services
{
    public interface IReaderService
    {
        Task<Domain.Models.Reader> EnsureExists(string readerId, CancellationToken ct = default);
        Task EnsureCanDelete(string readerId, CancellationToken ct = default);
    }
}
