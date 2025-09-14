using System.Threading;
using System.Threading.Tasks;

namespace Application.Books.Services
{
    public interface IBookService
    {
        /// <summary>
        /// Ensures the book exists and returns it. Throws DomainException if not found.
        /// </summary>
        Task<Domain.Models.Book> EnsureExistsAsync(string bookId, CancellationToken ct = default);
        Task DecreaseCopiesOrThrowAsync(string bookId, CancellationToken ct = default);
        Task RestoreCopiesAsync(string bookId, CancellationToken ct = default);
    }
}
