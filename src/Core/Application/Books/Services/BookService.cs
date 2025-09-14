using Application.Interfaces;
using Domain.Common;

namespace Application.Books.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;

        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<Domain.Models.Book> EnsureExistsAsync(string bookId, CancellationToken ct = default)
        {
            var book = await _bookRepository.GetByIdAsync(bookId, ct);
            if (book == null) throw new DomainException("Book not found.");
            return book;
        }

        public async Task DecreaseCopiesOrThrowAsync(string bookId, CancellationToken ct = default)
        {
            var changed = await _bookRepository.TryChangeCopiesAsync(bookId, -1, ct);
            if (!changed) throw new DomainException("No copies available for the requested book.");
        }

        public async Task RestoreCopiesAsync(string bookId, CancellationToken ct = default)
        {
            var changed = await _bookRepository.TryChangeCopiesAsync(bookId, +1, ct);
            if (!changed) throw new DomainException("Failed to restore book copies after loan creation failure.");
        }
    }
}
