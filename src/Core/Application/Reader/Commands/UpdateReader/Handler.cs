using Application.Readers.Mappers;
using Application.Readers.Services;
using FluentValidation;

namespace UpdateReader
{
    public class UpdateReaderCommandHandler : IUpdateReaderCommandHandler
    {
        private readonly IReaderService _readerService;
        private readonly IValidator<UpdateReaderCommandInput> _validator;

        public UpdateReaderCommandHandler(IReaderService readerService, IValidator<UpdateReaderCommandInput> validator)
        {
            _readerService = readerService;
            _validator = validator;
        }

        public async Task<UpdateReaderCommandOutput> Handle(UpdateReaderCommandInput input, CancellationToken ct = default)
        {
            await _validator.ValidateAndThrowAsync(input, ct);
            var updated = await _readerService.UpdateReader(input, ct);
            return updated.ToUpdateReaderOutput();
        }
    }
}
