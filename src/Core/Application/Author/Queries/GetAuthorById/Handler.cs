using Application.Interfaces;

namespace GetAuthorById
{
    public class GetAuthorByIdQueryHandler : IGetAuthorByIdQueryHandler
    {
        private readonly IAuthorRepository _authorRepository;

        public GetAuthorByIdQueryHandler(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        public async Task<GetAuthorByIdQueryOutput?> HandleAsync(GetAuthorByIdQueryInput query, CancellationToken ct = default)
        {
            var author = await _authorRepository.GetByIdAsync(query.Id, ct);
            if (author is null) return null;

            return new GetAuthorByIdQueryOutput
            {
                Id = author.Id,
                Name = author.Name,
                Bio = author.Bio,
                Nationality = string.IsNullOrWhiteSpace(author.Nationality) ? null : author.Nationality,
                BirthDate = author.BirthDate,
                DeathDate = author.DeathDate,
                Genres = author.Genres == null ? Array.Empty<string>() : Enumerable.ToArray(author.Genres)
            };
        }
    }
}
