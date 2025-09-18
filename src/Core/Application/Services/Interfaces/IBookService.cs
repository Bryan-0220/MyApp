using Domain.Models;
using Domain.Results;
using UpdateBook;

namespace Application.Books.Services
{
    public interface IBookService
    {
        Task<Book> GetBookOrThrow(string bookId, CancellationToken ct = default);
        Task DecreaseCopiesOrThrow(string bookId, CancellationToken ct = default);
        Task RestoreCopies(string bookId, CancellationToken ct = default);

        Task EnsureCanDelete(string bookId, CancellationToken ct = default);
        Task<Result<Book>> DeleteBook(string bookId, CancellationToken ct = default);
        Task<Book> UpdateBook(UpdateBookCommandInput input, CancellationToken ct = default);
    }
}
