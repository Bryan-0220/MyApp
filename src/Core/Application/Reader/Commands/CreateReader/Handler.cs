using FluentValidation;
using Application.Readers.Mappers;
using Application.Readers.Services;

namespace CreateReader
{
    public class CreateReaderCommandHandler : ICreateReaderCommandHandler
    {
        private readonly IReaderService _readerService;
        private readonly IValidator<CreateReaderCommandInput> _validator;

        public CreateReaderCommandHandler(IReaderService readerService, IValidator<CreateReaderCommandInput> validator)
        {
            _validator = validator;
            _readerService = readerService;
        }

        public async Task<CreateReaderCommandOutput> Handle(CreateReaderCommandInput input, CancellationToken ct = default)
        {
            await _validator.ValidateAndThrowAsync(input, ct);
            var created = await _readerService.CreateReader(input.ToData(), ct);
            return created.ToCreateReaderOutput();
        }
    }
}
