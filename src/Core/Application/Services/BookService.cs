using Application.Interfaces;
using Application.Filters;
using Domain.Results;
using Domain.Common;
using Domain.Models;
using UpdateBook;

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

        public async Task<Book> UpdateBook(UpdateBookCommandInput input, CancellationToken ct = default)
        {
            var existing = await _bookRepository.GetById(input.Id, ct);
            if (existing is null) throw new DomainException("Book not found");

            applyAttributes(input, existing);

            await _bookRepository.Update(existing, ct);
            return existing;
        }

        private static void applyAttributes(UpdateBookCommandInput input, Book existing)
        {
            if (!string.IsNullOrWhiteSpace(input.Title) && input.Title != "string")
                existing.SetTitle(input.Title!.Trim());

            if (!string.IsNullOrWhiteSpace(input.AuthorId) && input.AuthorId != "string")
                existing.SetAuthor(input.AuthorId!.Trim());

            if (!string.IsNullOrWhiteSpace(input.ISBN) && input.ISBN != "string")
                existing.SetIsbn(input.ISBN!.Trim());

            if (input.PublishedYear.HasValue && input.PublishedYear.Value != -1)
                existing.SetPublishedYear(input.PublishedYear);

            if (input.CopiesAvailable.HasValue && input.CopiesAvailable.Value != -1)
                existing.SetCopiesAvailable(input.CopiesAvailable.Value);

            if (!string.IsNullOrWhiteSpace(input.Genre) && input.Genre != "string")
                existing.SetGenre(input.Genre!.Trim());
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
