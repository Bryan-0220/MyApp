using Application.Interfaces;
using FluentValidation;
using Application.Readers.Services;

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

        public async Task<DeleteReaderCommandOutput> HandleAsync(DeleteReaderCommandInput input, CancellationToken ct = default)
        {
            await _validator.ValidateAndThrowAsync(input, ct);

            var existing = await _readerRepository.GetByIdAsync(input.Id, ct);
            if (existing is null)
            {
                return new DeleteReaderCommandOutput
                {
                    Success = false,
                    Message = "Reader not found"
                };
            }

            await _readerService.EnsureCanDelete(existing.Id, ct);

            await _readerRepository.DeleteAsync(input.Id, ct);
            return new DeleteReaderCommandOutput
            {
                Success = true,
                Message = "Reader deleted"
            };
        }
    }
}
