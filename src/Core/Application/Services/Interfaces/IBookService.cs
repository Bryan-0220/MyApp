
namespace Application.Books.Services
{
    public interface IBookService
    {
        Task<Domain.Models.Book> EnsureExists(string bookId, CancellationToken ct = default);
        Task DecreaseCopiesOrThrow(string bookId, CancellationToken ct = default);
        Task RestoreCopies(string bookId, CancellationToken ct = default);

        Task EnsureCanDelete(string bookId, CancellationToken ct = default);
    }
}
