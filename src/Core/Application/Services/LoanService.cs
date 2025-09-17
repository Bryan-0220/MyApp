using Application.Books.Services;
using Application.Readers.Services;
using Application.Filters;
using Application.Interfaces;
using Domain.Common;
using Domain.Models;
using Domain.Results;

namespace Application.Loans.Services
{
    public class LoanService : ILoanService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IBookService _bookService;
        private readonly IReaderService _readerService;
        private readonly IBookRepository _bookRepository;

        public LoanService(ILoanRepository loanRepository, IBookRepository bookRepository, IBookService bookService, IReaderService readerService)
        {
            _loanRepository = loanRepository;
            _bookRepository = bookRepository;
            _bookService = bookService;
            _readerService = readerService;
        }

        public async Task EnsureNoDuplicateLoan(string bookId, string readerId, CancellationToken ct = default)
        {
            var filter = new LoanFilter
            {
                BookId = bookId,
                UserId = readerId,
                Returned = false
            };

            var loans = await _loanRepository.Filter(filter, ct);
            if (loans != null && loans.Any())
                throw new DomainException("Reader already has this book on loan.");
        }

        public async Task<Result<Loan>> DeleteLoan(string loanId, CancellationToken ct = default)
        {
            var loan = await _loanRepository.GetById(loanId, ct);
            if (loan is null)
                return Result<Loan>.Fail("Loan not found.");

            if (!loan.CanBeDeleted(out var reason))
                return Result<Loan>.Fail(reason ?? "Loan cannot be deleted for an unspecified reason.");

            var restoreResult = await TryRestoreCopies(loan.BookId, ct);
            if (!restoreResult.Success)
                return Result<Loan>.Fail(restoreResult.Message);

            await _loanRepository.Delete(loanId, ct);
            return Result<Loan>.Ok(loan, "Loan deleted.");
        }

        public async Task<Loan> CreateLoan(LoanData data, CancellationToken ct = default)
        {
            var book = await EnsureCreationBusinessRules(data, ct);

            await _bookService.DecreaseCopiesOrThrow(data.BookId, ct);
            var loan = Loan.Create(data);

            var created = await _loanRepository.Create(loan, ct);
            return created;
        }

        private async Task<Book> EnsureCreationBusinessRules(LoanData data, CancellationToken ct)
        {
            var book = await _bookService.GetBookOrThrow(data.BookId!, ct);
            await _readerService.EnsureExists(data.ReaderId!, ct);
            await EnsureNoDuplicateLoan(data.BookId!, data.ReaderId!, ct);
            book.EnsureHasAvailableCopies();
            return book;
        }

        private async Task<(bool Success, string Message)> TryRestoreCopies(string bookId, CancellationToken ct)
        {
            try
            {
                await _bookService.RestoreCopies(bookId, ct);
                return (true, string.Empty);
            }
            catch (DomainException ex)
            {
                return (false, ex.Message);
            }
            catch (Exception)
            {
                return (false, "Unexpected error restoring book copies.");
            }
        }
    }
}
