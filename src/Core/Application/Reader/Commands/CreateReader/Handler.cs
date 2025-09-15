using Domain.Models;
using Domain.Common;
using Application.Interfaces;
using FluentValidation;
using Application.Readers.Mappers;

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

            Reader reader;
            try
            {
                reader = Reader.Create(input.ToData());
            }
            catch (DomainException ex)
            {
                throw new InvalidOperationException(ex.Message);
            }

            var created = await _readerRepository.CreateAsync(reader, ct);

            return created.ToCreateReaderOutput();
        }
    }
}
