using Application.Filters;
using Application.Interfaces;
using Domain.Common;

namespace Application.Loans.Services
{
    public class LoanService : ILoanService
    {
        private readonly ILoanRepository _loanRepository;

        public LoanService(ILoanRepository loanRepository)
        {
            _loanRepository = loanRepository;
        }

        public async Task EnsureNoDuplicateLoanAsync(string bookId, string readerId, CancellationToken ct = default)
        {
            var filter = new LoanFilter
            {
                BookId = bookId,
                UserId = readerId,
                Returned = false
            };

            var loans = await _loanRepository.FilterAsync(filter, ct);
            if (loans != null && loans.Any())
            {
                throw new DomainException("Reader already has this book on loan.");
            }
        }
    }
}
