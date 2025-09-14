using Application.Interfaces;
using Domain.Common;
using Domain.Models;

namespace Application.Books.Services
{
    public class BookDeletionService : IBookDeletionService
    {
        private readonly ILoanRepository _loanRepository;

        public BookDeletionService(ILoanRepository loanRepository)
        {
            _loanRepository = loanRepository;
        }

        public async Task EnsureCanDeleteAsync(string bookId, CancellationToken ct = default)
        {
            var filter = new Application.Filters.LoanFilter
            {
                BookId = bookId,
                Returned = false
            };

            var loans = await _loanRepository.FilterAsync(filter, ct);
            if (loans != null && loans.Any())
            {
                throw new Domain.Common.DomainException("Book cannot be deleted while it has active loans.");
            }
        }
    }
}
