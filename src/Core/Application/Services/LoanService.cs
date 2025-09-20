using Application.Books.Services;
using Application.Readers.Services;
using Application.Filters;
using Application.Interfaces;
using Domain.Common;
using Domain.Models;
using Domain.Results;
using UpdateLoan;

namespace Application.Loans.Services
{
    public class LoanService : ILoanService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IBookService _bookService;
        private readonly IReaderService _readerService;

        public LoanService(ILoanRepository loanRepository, IBookService bookService, IReaderService readerService)
        {
            _loanRepository = loanRepository;
            _bookService = bookService;
            _readerService = readerService;
        }

        public async Task EnsureNoDuplicateLoan(string bookId, string readerId, string? excludeLoanId = null, CancellationToken ct = default)
        {
            var filter = new LoanFilter
            {
                BookId = bookId,
                UserId = readerId,
                Returned = false
            };

            var loans = await _loanRepository.Filter(filter, ct);
            if (loans != null)
            {
                var hasOther = loans.Any(l => !string.Equals(l.Id, excludeLoanId, StringComparison.OrdinalIgnoreCase));
                if (hasOther)
                    throw new DuplicateException("Reader already has this book on loan.");
            }
        }

        public async Task<Result<Loan>> DeleteLoan(string loanId, CancellationToken ct = default)
        {
            var loan = await _loanRepository.GetById(loanId, ct);
            if (loan is null)
                return Result<Loan>.Fail("Loan not found.");

            if (!loan.CanBeDeleted(out var reason))
                return Result<Loan>.Fail(reason ?? "Loan cannot be deleted for an unspecified reason.");

            var restoreResult = await tryRestoreCopies(loan.BookId, ct);
            if (!restoreResult.Success)
                return Result<Loan>.Fail(restoreResult.Message);

            await _loanRepository.Delete(loanId, ct);
            return Result<Loan>.Ok(loan, "Loan deleted.");
        }

        public async Task<Loan> CreateLoan(LoanData data, CancellationToken ct = default)
        {
            await EnsureCreationBusinessRules(data, ct);

            await _bookService.DecreaseCopiesOrThrow(data.BookId, ct);
            var loan = Loan.Create(data);

            var created = await _loanRepository.Create(loan, ct);
            return created;
        }

        private async Task EnsureCreationBusinessRules(LoanData data, CancellationToken ct)
        {
            var book = await _bookService.GetBookOrThrow(data.BookId!, ct);
            await _readerService.EnsureExists(data.ReaderId!, ct);
            await EnsureNoDuplicateLoan(data.BookId!, data.ReaderId!, null, ct);
            book.EnsureHasAvailableCopies();
        }

        private async Task<(bool Success, string Message)> tryRestoreCopies(string bookId, CancellationToken ct)
        {
            try
            {
                await _bookService.RestoreCopies(bookId, ct);
                return (true, string.Empty);
            }
            catch (BusinessRuleException ex)
            {
                return (false, ex.Message);
            }
            catch (DuplicateException ex)
            {
                return (false, ex.Message);
            }
            catch (Exception)
            {
                return (false, "Unexpected error restoring book copies.");
            }
        }

        public async Task<Loan> UpdateLoan(UpdateLoanCommandInput input, CancellationToken ct = default)
        {
            var existing = await _loanRepository.GetById(input.Id, ct);
            if (existing is null) throw new NotFoundException("Loan not found");

            static bool IsMeaningful(string? s) => !string.IsNullOrWhiteSpace(s);
            if (IsMeaningful(input.BookId) || IsMeaningful(input.ReaderId))
            {
                var targetBookId = IsMeaningful(input.BookId) ? input.BookId!.Trim() : existing.BookId;
                var targetReaderId = IsMeaningful(input.ReaderId) ? input.ReaderId!.Trim() : existing.ReaderId;

                await _bookService.GetBookOrThrow(targetBookId, ct);
                await _readerService.EnsureExists(targetReaderId, ct);

                await EnsureNoDuplicateLoan(targetBookId, targetReaderId, existing.Id, ct);
            }

            ApplyUpdates(input, existing);
            await _loanRepository.Update(existing, ct);
            return existing;
        }

        private static void ApplyUpdates(UpdateLoanCommandInput input, Loan existing)
        {
            if (!string.IsNullOrWhiteSpace(input.BookId))
                existing.BookId = input.BookId!.Trim();

            if (!string.IsNullOrWhiteSpace(input.ReaderId))
                existing.ReaderId = input.ReaderId!.Trim();

            if (input.LoanDate.HasValue)
                existing.LoanDate = input.LoanDate.Value;

            if (input.DueDate.HasValue)
                existing.DueDate = input.DueDate.Value;

            if (input.ReturnedDate.HasValue)
                existing.ReturnedDate = input.ReturnedDate.Value;

            if (!string.IsNullOrWhiteSpace(input.Status))
            {
                if (Enum.TryParse<LoanStatus>(input.Status, true, out var st))
                    existing.Status = st;
            }
        }
    }
}
