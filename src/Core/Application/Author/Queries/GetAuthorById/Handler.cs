using Application.Interfaces;
using Application.Authors.Mappers;
using Domain.Results;

namespace GetAuthorById
{
    public class GetAuthorByIdQueryHandler : IGetAuthorByIdQueryHandler
    {
        private readonly IAuthorRepository _authorRepository;

        public GetAuthorByIdQueryHandler(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        public async Task<Result<GetAuthorByIdQueryOutput>> Handle(GetAuthorByIdQueryInput query, CancellationToken ct = default)
        {
            var author = await _authorRepository.GetById(query.Id, ct);
            if (author is null) return Result<GetAuthorByIdQueryOutput>.Fail("Author not found");
            return Result<GetAuthorByIdQueryOutput>.Ok(author.ToGetAuthorByIdOutput());
        }
    }
}
