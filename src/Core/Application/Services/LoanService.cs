using Application.Filters;
using Application.Interfaces;
using Domain.Common;
using Domain.Models;
using Application.Books.Services;

namespace Application.Loans.Services
{
    public class LoanService : ILoanService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly Application.Books.Services.IBookService _bookService;

        public LoanService(ILoanRepository loanRepository, IBookService bookService)
        {
            _loanRepository = loanRepository;
            _bookService = bookService;
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
            {
                throw new DomainException("Reader already has this book on loan.");
            }
        }




        public Task<bool> EnsureCanDelete(Loan loan, CancellationToken ct = default)
        {
            if (loan is null) throw new DomainException("Loan is null");

            if (loan.Status != LoanStatus.Returned)
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        public async Task HandlePostDelete(Loan loan, CancellationToken ct = default)
        {
            if (loan is null) throw new DomainException("Loan is null");

            try
            {
                await _bookService.RestoreCopies(loan.BookId, ct);
            }
            catch (System.Exception ex)
            {
                throw new DomainException($"Failed to restore book copies for book {loan.BookId}: {ex.Message}");
            }
        }
    }
}
