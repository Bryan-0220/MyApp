using Application.Interfaces;

namespace GetAllAuthors
{
    public class GetAllAuthorsQueryHandler : IGetAllAuthorsQueryHandler
    {
        private readonly IAuthorRepository _authorRepository;

        public GetAllAuthorsQueryHandler(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        public async Task<IEnumerable<GetAllAuthorsQueryOutput>> HandleAsync(GetAllAuthorsQueryInput query, CancellationToken ct = default)
        {
            var authors = await _authorRepository.GetAllAsync(ct);

            var projected = authors.Select(a => new GetAllAuthorsQueryOutput
            {
                Id = a.Id,
                Name = a.Name,
                Bio = a.Bio,
                Nationality = string.IsNullOrWhiteSpace(a.Nationality) ? null : a.Nationality,
                BirthDate = a.BirthDate,
                DeathDate = a.DeathDate,
                Genres = a.Genres == null ? System.Array.Empty<string>() : System.Linq.Enumerable.ToArray(a.Genres)
            });

            return projected;
        }
    }
}
