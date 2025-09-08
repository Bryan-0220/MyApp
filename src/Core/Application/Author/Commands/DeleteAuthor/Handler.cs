using Application.Interfaces;
using FluentValidation;

namespace DeleteAuthor
{
    public class DeleteAuthorCommandHandler : IDeleteAuthorCommandHandler
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IValidator<DeleteAuthorCommandInput> _validator;

        public DeleteAuthorCommandHandler(IAuthorRepository authorRepository, IValidator<DeleteAuthorCommandInput> validator)
        {
            _authorRepository = authorRepository;
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

            await _authorRepository.DeleteAsync(input.Id, ct);
            return new DeleteAuthorCommandOutput
            {
                Success = true,
                Message = "Author deleted"
            };
        }
    }
}
