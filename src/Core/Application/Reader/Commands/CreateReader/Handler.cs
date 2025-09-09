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
                FullName = input.FullName,
                Email = input.Email,
                MembershipDate = input.MembershipDate ?? System.DateTime.UtcNow
            };

            var created = await _readerRepository.CreateAsync(reader, ct);

            return new CreateReaderCommandOutput
            {
                Id = created.Id,
                FullName = created.FullName,
                Email = created.Email,
                MembershipDate = created.MembershipDate
            };
        }
    }
}
