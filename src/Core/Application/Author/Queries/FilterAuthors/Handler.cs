using Application.Interfaces;
using FluentValidation;

namespace FilterAuthors
{
    public class FilterAuthorsQueryHandler : IFilterAuthorsQueryHandler
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IValidator<FilterAuthorsQueryInput> _validator;

        public FilterAuthorsQueryHandler(IAuthorRepository authorRepository, IValidator<FilterAuthorsQueryInput> validator)
        {
            _authorRepository = authorRepository;
            _validator = validator;
        }

        public async Task<IEnumerable<FilterAuthorsQueryOutput>> HandleAsync(FilterAuthorsQueryInput input, CancellationToken ct = default)
        {
            await _validator.ValidateAndThrowAsync(input, ct);

            var authors = await _authorRepository.FilterAsync(input.Name, input.Nationality, input.Genres, ct);

            return authors.Select(a => new FilterAuthorsQueryOutput
            {
                Id = a.Id,
                Name = a.Name,
                Bio = a.Bio,
                Nationality = string.IsNullOrWhiteSpace(a.Nationality) ? null : a.Nationality,
                BirthDate = a.BirthDate,
                DeathDate = a.DeathDate,
                Genres = a.Genres == null ? System.Array.Empty<string>() : System.Linq.Enumerable.ToArray(a.Genres)
            });
        }
    }
}
