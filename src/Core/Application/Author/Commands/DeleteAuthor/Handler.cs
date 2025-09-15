using Application.Interfaces;
using Application.Authors.Mappers;
using FluentValidation;
using Application.Authors.Services;
using Domain.Common;

namespace DeleteAuthor
{
    public class DeleteAuthorCommandHandler : IDeleteAuthorCommandHandler
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IAuthorService _deletionService;
        private readonly IValidator<DeleteAuthorCommandInput> _validator;

        public DeleteAuthorCommandHandler(IAuthorRepository authorRepository, IAuthorService deletionService, IValidator<DeleteAuthorCommandInput> validator)
        {
            _authorRepository = authorRepository;
            _deletionService = deletionService;
            _validator = validator;
        }

        public async Task<DeleteAuthorCommandOutput> Handle(DeleteAuthorCommandInput input, CancellationToken ct = default)
        {
            await _validator.ValidateAndThrowAsync(input, ct);

            var existing = await _authorRepository.GetById(input.Id, ct);
            if (existing is null)
            {
                return (null as Domain.Models.Author).ToDeleteOutput(false, "Author not found");
            }

            try
            {
                await _deletionService.EnsureCanDelete(input.Id, ct);
            }
            catch (DomainException dex)
            {
                return existing.ToDeleteOutput(false, dex.Message);
            }

            await _authorRepository.Delete(input.Id, ct);
            return existing.ToDeleteOutput(true, "Author deleted");
        }
    }
}
