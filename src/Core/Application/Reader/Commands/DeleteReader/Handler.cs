using Application.Interfaces;
using FluentValidation;
using Application.Readers.Services;
using Application.Readers.Mappers;

namespace DeleteReader
{
    public class DeleteReaderCommandHandler : IDeleteReaderCommandHandler
    {
        private readonly IReaderRepository _readerRepository;
        private readonly IReaderService _readerService;
        private readonly IValidator<DeleteReaderCommandInput> _validator;

        public DeleteReaderCommandHandler(IReaderRepository readerRepository, IReaderService readerService, IValidator<DeleteReaderCommandInput> validator)
        {
            _readerRepository = readerRepository;
            _readerService = readerService;
            _validator = validator;
        }

        public async Task<DeleteReaderCommandOutput> Handle(DeleteReaderCommandInput input, CancellationToken ct = default)
        {
            await _validator.ValidateAndThrowAsync(input, ct);
            var result = await _readerService.DeleteReader(input.Id, ct);
            return result.ToDeleteReaderOutput();
        }
    }
}
