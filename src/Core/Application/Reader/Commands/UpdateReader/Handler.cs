using Application.Interfaces;
using Domain.Models;
using FluentValidation;
using Application.Readers.Mappers;

namespace UpdateReader
{
    public class UpdateReaderCommandHandler : IUpdateReaderCommandHandler
    {
        private readonly IReaderRepository _readerRepository;
        private readonly IValidator<UpdateReaderCommandInput> _validator;

        public UpdateReaderCommandHandler(IReaderRepository readerRepository, IValidator<UpdateReaderCommandInput> validator)
        {
            _readerRepository = readerRepository;
            _validator = validator;
        }

        public async Task<UpdateReaderCommandOutput?> HandleAsync(UpdateReaderCommandInput input, CancellationToken ct = default)
        {
            await _validator.ValidateAndThrowAsync(input, ct);

            var existing = await _readerRepository.GetByIdAsync(input.Id, ct);
            if (existing is null) return null;

            try
            {
                applyAttributes(input, existing);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }

            await _readerRepository.UpdateAsync(existing, ct);

            return existing.ToUpdateReaderOutput();
        }

        private static void applyAttributes(UpdateReaderCommandInput input, Reader existing)
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
    }
}
