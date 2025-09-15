using Application.Interfaces;
using Application.Authors.Mappers;

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

            return author.ToGetAuthorByIdOutput();
        }
    }
}
