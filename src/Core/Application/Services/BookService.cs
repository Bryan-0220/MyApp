using Application.Interfaces;
using Application.Filters;
using Domain.Results;
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

        public async Task<Result<Book>> DeleteBook(string bookId, CancellationToken ct = default)
        {
            var book = await _bookRepository.GetById(bookId, ct);
            if (book is null)
                return Result<Book>.Fail("Book not found.");

            var ensureResult = await TryEnsureCanDelete(bookId, ct);
            if (!ensureResult.Success)
                return Result<Book>.Fail(ensureResult.Message);

            await _bookRepository.Delete(bookId, ct);
            return Result<Book>.Ok(book, "Book deleted.");
        }

        private async Task<(bool Success, string Message)> TryEnsureCanDelete(string bookId, CancellationToken ct)
        {
            try
            {
                await EnsureCanDelete(bookId, ct);
                return (true, string.Empty);
            }
            catch (DomainException ex)
            {
                return (false, ex.Message);
            }
            catch (Exception)
            {
                return (false, "Unexpected error validating book deletion.");
            }
        }

        public async Task<Book> GetBookOrThrow(string bookId, CancellationToken ct = default)
        {
            var book = await _bookRepository.GetById(bookId, ct);
            if (book == null) throw new DomainException("Book not found.");
            return book;
        }

        public async Task DecreaseCopiesOrThrow(string bookId, CancellationToken ct = default)
        {
            var changed = await _bookRepository.TryChangeCopies(bookId, -1, ct);
            if (!changed) throw new DomainException("No copies available for the requested book.");
        }

        public async Task RestoreCopies(string bookId, CancellationToken ct = default)
        {
            var changed = await _bookRepository.TryChangeCopies(bookId, +1, ct);
            if (!changed) throw new DomainException("Failed to restore book copies after loan creation failure.");
        }





        public async Task EnsureCanDelete(string bookId, CancellationToken ct = default)
        {
            var filter = new LoanFilter
            {
                BookId = bookId,
                Returned = false
            };

            var loans = await _loanRepository.Filter(filter, ct);
            if (loans != null && loans.Any())
            {
                throw new DomainException("Book cannot be deleted while it has active loans.");
            }
        }
    }
}
