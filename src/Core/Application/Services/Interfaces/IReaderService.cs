using Domain.Models;
using Domain.Results;
using UpdateReader;

namespace Application.Readers.Services
{
    public interface IReaderService
    {
        Task EnsureCanDelete(string readerId, CancellationToken ct = default);
        Task<Result<Reader>> DeleteReader(string readerId, CancellationToken ct = default);
        Task EnsureExists(string readerId, CancellationToken ct = default);
        Task EnsureEmailNotInUse(string email, CancellationToken ct = default);
        Task<Reader> CreateReader(ReaderData input, CancellationToken ct = default);
        Task<Reader> UpdateReader(UpdateReaderCommandInput input, CancellationToken ct = default);
    }
}
