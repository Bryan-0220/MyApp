using Application.Interfaces;
using Domain.Common;
using Domain.Models;
using Application.Filters;

namespace Application.Readers.Services
{
    public class ReaderService : IReaderService
    {
        private readonly IReaderRepository _readerRepository;
        private readonly ILoanRepository _loanRepository;

        public ReaderService(IReaderRepository readerRepository, ILoanRepository loanRepository)
        {
            _readerRepository = readerRepository;
            _loanRepository = loanRepository;
        }

        public async Task<Reader> EnsureExists(string readerId, CancellationToken ct = default)
        {
            var reader = await _readerRepository.GetByIdAsync(readerId, ct);
            if (reader == null) throw new DomainException("Reader not found.");
            return reader;
        }




        public async Task EnsureCanDelete(string readerId, CancellationToken ct = default)
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
