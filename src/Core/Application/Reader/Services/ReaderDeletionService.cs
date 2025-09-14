using Application.Interfaces;
using Application.Filters;
using Domain.Common;

namespace Application.Readers.Services
{
    public class ReaderDeletionService : IReaderDeletionService
    {
        private readonly ILoanRepository _loanRepository;

        public ReaderDeletionService(ILoanRepository loanRepository)
        {
            _loanRepository = loanRepository;
        }

        public async Task EnsureCanDeleteAsync(string readerId, CancellationToken ct = default)
        {
            var filter = new LoanFilter
            {
                UserId = readerId,
                Returned = false
            };

            var loans = await _loanRepository.FilterAsync(filter, ct);
            if (loans != null && loans.Any())
            {
                throw new DomainException("Reader cannot be deleted while has active loans.");
            }
        }
    }
}
