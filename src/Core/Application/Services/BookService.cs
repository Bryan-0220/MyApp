using Application.Interfaces;
using Domain.Common;
using Domain.Models;

namespace Application.Books.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly ILoanRepository _loanRepository;

        public BookService(IBookRepository bookRepository, ILoanRepository loanRepository)
        {
            _bookRepository = bookRepository;
            _loanRepository = loanRepository;
        }

        public async Task<Book> EnsureExists(string bookId, CancellationToken ct = default)
        {
            var book = await _bookRepository.GetByIdAsync(bookId, ct);
            if (book == null) throw new DomainException("Book not found.");
            return book;
        }

        public async Task DecreaseCopiesOrThrow(string bookId, CancellationToken ct = default)
        {
            var changed = await _bookRepository.TryChangeCopiesAsync(bookId, -1, ct);
            if (!changed) throw new DomainException("No copies available for the requested book.");
        }

        public async Task RestoreCopies(string bookId, CancellationToken ct = default)
        {
            var changed = await _bookRepository.TryChangeCopiesAsync(bookId, +1, ct);
            if (!changed) throw new DomainException("Failed to restore book copies after loan creation failure.");
        }





        public async Task EnsureCanDelete(string bookId, CancellationToken ct = default)
        {
            var filter = new Application.Filters.LoanFilter
            {
                BookId = bookId,
                Returned = false
            };

            var loans = await _loanRepository.FilterAsync(filter, ct);
            if (loans != null && loans.Any())
            {
                throw new DomainException("Book cannot be deleted while it has active loans.");
            }
        }
    }
}
