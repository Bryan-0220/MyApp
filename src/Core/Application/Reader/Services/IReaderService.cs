using System.Threading;
using System.Threading.Tasks;

namespace Application.Readers.Services
{
    public interface IReaderService
    {
        /// <summary>
        /// Ensures the reader exists and returns it. Throws DomainException if not found.
        /// </summary>
        Task<Domain.Models.Reader> EnsureExistsAsync(string readerId, CancellationToken ct = default);
    }
}
