using Domain.Models;
using Domain.Results;

namespace Application.Readers.Services
{
    public interface IReaderService
    {
        Task<Reader> GetReaderOrThrow(string readerId, CancellationToken ct = default);
        Task EnsureCanDelete(string readerId, CancellationToken ct = default);
        Task<Result<Reader>> DeleteReader(string readerId, CancellationToken ct = default);
        Task EnsureExists(string readerId, CancellationToken ct = default);
    }
}
