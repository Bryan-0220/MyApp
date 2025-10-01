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
            if (reader == null) throw new NotFoundException("Reader not found.");
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
                return (false, "Unexpected error validating reader deletion.");
            }
        }

        public async Task<Reader> CreateReader(ReaderData input, CancellationToken ct = default)
        {
            await EnsureEmailNotInUse(input.Email!, null, ct);
            var reader = Reader.Create(input);
            var created = await _readerRepository.Create(reader, ct);
            return created;
        }

        public async Task<Reader> UpdateReader(UpdateReaderCommandInput input, CancellationToken ct = default)
        {
            var existing = await _readerRepository.GetById(input.Id, ct);
            if (existing is null) throw new NotFoundException("Reader not found");

            static bool IsMeaningful(string? s) => !string.IsNullOrWhiteSpace(s) && s != "string";
            if (IsMeaningful(input.Email))
            {
                var normalized = input.Email!.Trim();
                await EnsureEmailNotInUse(normalized, input.Id, ct);
            }

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

        public async Task EnsureEmailNotInUse(string email, string? excludeId = null, CancellationToken ct = default)
        {
            var filter = new ReaderFilter { Email = email };
            var results = await _readerRepository.Filter(filter, ct);
            if (results != null && results.Any())
            {
                // Si excludeId es null (creando nuevo reader), cualquier resultado significa duplicado
                if (excludeId == null)
                {
                    throw new DuplicateException("Email is already registered for another reader.");
                }

                // Si excludeId no es null (actualizando reader), verificar si hay otros readers con el mismo email
                var hasOther = results.Any(r => !string.Equals(r.Id, excludeId, StringComparison.OrdinalIgnoreCase));
                if (hasOther)
                {
                    throw new DuplicateException("Email is already registered for another reader.");
                }
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
                throw new BusinessRuleException("Reader cannot be deleted while has active loans.");
            }
        }
    }
}
