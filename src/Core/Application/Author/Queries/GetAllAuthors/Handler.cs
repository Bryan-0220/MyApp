using Application.Interfaces;
using Application.Authors.Mappers;

namespace GetAllAuthors
{
    public class GetAllAuthorsQueryHandler : IGetAllAuthorsQueryHandler
    {
        private readonly IAuthorRepository _authorRepository;

        public GetAllAuthorsQueryHandler(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        public async Task<IEnumerable<GetAllAuthorsQueryOutput>> Handle(GetAllAuthorsQueryInput query, CancellationToken ct = default)
        {
            var authors = await _authorRepository.GetAll(ct);

            var projected = authors.Select(a => a.ToGetAllAuthorsOutput());

            return projected;
        }
    }
}
