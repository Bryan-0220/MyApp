using Application.Interfaces;
using Domain.Common;
using Domain.Results;
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
            var reader = await _readerRepository.GetById(readerId, ct);
            if (reader == null) throw new DomainException("Reader not found.");
            return reader;
        }


        public async Task<Result<Reader>> DeleteReader(string readerId, CancellationToken ct = default)
        {
            var reader = await _readerRepository.GetById(readerId, ct);
            if (reader is null)
                return Result<Reader>.Fail("Reader not found.");

            var ensureResult = await TryEnsureCanDelete(readerId, ct);
            if (!ensureResult.Success)
                return Result<Reader>.Fail(ensureResult.Message);

            await _readerRepository.Delete(readerId, ct);
            return Result<Reader>.Ok(reader, "Reader deleted.");
        }

        private async Task<(bool Success, string Message)> TryEnsureCanDelete(string readerId, CancellationToken ct)
        {
            try
            {
                await EnsureCanDelete(readerId, ct);
                return (true, string.Empty);
            }
            catch (DomainException ex)
            {
                return (false, ex.Message);
            }
            catch (Exception)
            {
                return (false, "Unexpected error validating reader deletion.");
            }
        }




        public async Task EnsureCanDelete(string readerId, CancellationToken ct = default)
        {
            var filter = new LoanFilter
            {
                UserId = readerId,
                Returned = false
            };

            var loans = await _loanRepository.Filter(filter, ct);
            if (loans != null && loans.Any())
            {
                throw new DomainException("Reader cannot be deleted while has active loans.");
            }
        }
    }
}
