using Application.Interfaces;
using FluentValidation;
using Application.Authors.Services;
using Domain.Common;

namespace DeleteAuthor
{
    public class DeleteAuthorCommandHandler : IDeleteAuthorCommandHandler
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IAuthorDeletionService _deletionService;
        private readonly IValidator<DeleteAuthorCommandInput> _validator;

        public DeleteAuthorCommandHandler(IAuthorRepository authorRepository, IAuthorDeletionService deletionService, IValidator<DeleteAuthorCommandInput> validator)
        {
            _authorRepository = authorRepository;
            _deletionService = deletionService;
            _validator = validator;
        }

        public async Task<DeleteAuthorCommandOutput> HandleAsync(DeleteAuthorCommandInput input, CancellationToken ct = default)
        {
            await _validator.ValidateAndThrowAsync(input, ct);

            var existing = await _authorRepository.GetByIdAsync(input.Id, ct);
            if (existing is null)
            {
                return new DeleteAuthorCommandOutput
                {
                    Success = false,
                    Message = "Author not found"
                };
            }

            try
            {
                await _deletionService.EnsureCanDeleteAsync(input.Id, ct);
            }
            catch (DomainException dex)
            {
                return new DeleteAuthorCommandOutput
                {
                    Success = false,
                    Message = dex.Message
                };
            }

            await _authorRepository.DeleteAsync(input.Id, ct);
            return new DeleteAuthorCommandOutput
            {
                Success = true,
                Message = "Author deleted"
            };
        }
    }
}
