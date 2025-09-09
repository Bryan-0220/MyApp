using Domain.Models;
using Application.Interfaces;
using FluentValidation;

namespace CreateReader
{
    public class CreateReaderCommandHandler : ICreateReaderCommandHandler
    {
        private readonly IReaderRepository _readerRepository;
        private readonly IValidator<CreateReaderCommandInput> _validator;

        public CreateReaderCommandHandler(IReaderRepository readerRepository, IValidator<CreateReaderCommandInput> validator)
        {
            _readerRepository = readerRepository;
            _validator = validator;
        }

        public async Task<CreateReaderCommandOutput> HandleAsync(CreateReaderCommandInput input, CancellationToken ct = default)
        {
            await _validator.ValidateAndThrowAsync(input, ct);

            var reader = new Reader
            {
                FirstName = input.FirstName,
                LastName = input.LastName,
                Email = input.Email,
                MembershipDate = input.MembershipDate ?? DateOnly.FromDateTime(DateTime.UtcNow)
            };

            var created = await _readerRepository.CreateAsync(reader, ct);

            return new CreateReaderCommandOutput
            {
                Id = created.Id,
                FirstName = created.FirstName,
                LastName = created.LastName,
                Email = created.Email,
                MembershipDate = created.MembershipDate
            };
        }
    }
}
