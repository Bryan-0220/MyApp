using Domain.Models;
using Domain.Common;
using Application.Interfaces;
using FluentValidation;

namespace CreateAuthor
{
    public class CreateAuthorCommandHandler : ICreateAuthorCommandHandler
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IValidator<CreateAuthorCommandInput> _validator;

        public CreateAuthorCommandHandler(IAuthorRepository authorRepository, IValidator<CreateAuthorCommandInput> validator)
        {
            _authorRepository = authorRepository;
            _validator = validator;
        }

        public async Task<CreateAuthorCommandOutput> HandleAsync(CreateAuthorCommandInput input, CancellationToken ct = default)
        {
            await _validator.ValidateAndThrowAsync(input, ct);

            Author author;
            try
            {
                author = Author.Create(input.Name, input.Bio, input.Nationality, input.BirthDate, input.DeathDate, input.Genres);
            }
            catch (DomainException ex)
            {
                throw new InvalidOperationException(ex.Message);
            }

            var created = await _authorRepository.CreateAsync(author, ct);

            return new CreateAuthorCommandOutput
            {
                Id = created.Id,
                Name = created.Name,
                Bio = created.Bio,
                Nationality = string.IsNullOrWhiteSpace(created.Nationality) ? null : created.Nationality,
                BirthDate = created.BirthDate,
                DeathDate = created.DeathDate,
                Genres = (created.Genres == null) ? System.Array.Empty<string>() : System.Linq.Enumerable.ToArray(created.Genres)
            };
        }
    }
}
