using Application.Interfaces;
using Domain.Common;
using Domain.Results;
using Domain.Models;
using Application.Filters;
using UpdateReader;

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

        public async Task EnsureExists(string readerId, CancellationToken ct = default)
        {
            var reader = await _readerRepository.GetById(readerId, ct);
            if (reader == null) throw new DomainException("Reader not found.");
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

        public async Task<Reader> CreateReader(ReaderData input, CancellationToken ct = default)
        {
            await EnsureEmailNotInUse(input.Email!, ct);
            var reader = Reader.Create(input);
            var created = await _readerRepository.Create(reader, ct);
            return created;
        }

        public async Task<Reader> UpdateReader(UpdateReaderCommandInput input, CancellationToken ct = default)
        {
            var existing = await _readerRepository.GetById(input.Id, ct);
            if (existing is null) throw new DomainException("Reader not found");

            ApplyAttributes(input, existing);

            await _readerRepository.Update(existing, ct);
            return existing;
        }

        private static void ApplyAttributes(UpdateReaderCommandInput input, Reader existing)
        {
            static bool IsMeaningful(string? s) => !string.IsNullOrWhiteSpace(s) && s != "string";

            if (IsMeaningful(input.FirstName))
                existing.FirstName = input.FirstName!.Trim();

            if (IsMeaningful(input.LastName))
                existing.LastName = input.LastName!.Trim();

            if (IsMeaningful(input.Email))
                existing.Email = input.Email!.Trim();

            if (input.MembershipDate.HasValue)
                existing.MembershipDate = input.MembershipDate.Value;
        }

        public async Task EnsureEmailNotInUse(string email, CancellationToken ct = default)
        {
            var filter = new ReaderFilter { Email = email };
            var results = await _readerRepository.Filter(filter, ct);
            if (results != null && results.Any())
                throw new DomainException("Email is already registered for another reader.");
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
