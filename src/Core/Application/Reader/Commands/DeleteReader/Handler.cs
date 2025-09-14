using Application.Interfaces;
using FluentValidation;
using Application.Readers.Services;

namespace DeleteReader
{
    public class DeleteReaderCommandHandler : IDeleteReaderCommandHandler
    {
        private readonly IReaderRepository _readerRepository;
        private readonly IReaderDeletionService _deletionService;
        private readonly IValidator<DeleteReaderCommandInput> _validator;

        public DeleteReaderCommandHandler(IReaderRepository readerRepository, IReaderDeletionService deletionService, IValidator<DeleteReaderCommandInput> validator)
        {
            _readerRepository = readerRepository;
            _deletionService = deletionService;
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

            await _deletionService.EnsureCanDeleteAsync(input.Id, ct);

            await _readerRepository.DeleteAsync(input.Id, ct);
            return new DeleteReaderCommandOutput
            {
                Success = true,
                Message = "Reader deleted"
            };
        }
    }
}
