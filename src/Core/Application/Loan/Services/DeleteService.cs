using Domain.Common;
using Domain.Models;
using Application.Books.Services;

namespace Application.Loans.Services
{
    public class DeleteService : IDeleteService
    {
        private readonly IBookService _bookService;

        public DeleteService(IBookService bookService)
        {
            _bookService = bookService ?? throw new System.ArgumentNullException(nameof(bookService));
        }

        public Task<bool> EnsureCanDeleteAsync(Loan loan, CancellationToken ct = default)
        {
            if (loan is null) throw new DomainException("Loan is null");

            // Only allow deletion when the loan is already returned.
            // Active or Overdue loans must be marked as Returned (via the proper return flow) before deletion.
            if (loan.Status != LoanStatus.Returned)
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        public async Task HandlePostDeleteAsync(Loan loan, CancellationToken ct = default)
        {
            if (loan is null) throw new DomainException("Loan is null");

            try
            {
                await _bookService.RestoreCopiesAsync(loan.BookId, ct);
            }
            catch (System.Exception ex)
            {
                throw new DomainException($"Failed to restore book copies for book {loan.BookId}: {ex.Message}");
            }
        }
    }
}
