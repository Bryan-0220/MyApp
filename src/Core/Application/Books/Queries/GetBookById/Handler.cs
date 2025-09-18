using Application.Interfaces;
using Application.Books.Mappers;
using Domain.Results;

namespace GetBookById
{
    public class GetBookByIdQueryHandler : IGetBookByIdQueryHandler
    {
        private readonly IBookRepository _bookRepository;

        public GetBookByIdQueryHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<Result<GetBookByIdQueryOutput>> Handle(GetBookByIdQueryInput query, CancellationToken ct = default)
        {
            var book = await _bookRepository.GetById(query.Id, ct);
            if (book is null) return Result<GetBookByIdQueryOutput>.Fail("Book not found");
            return Result<GetBookByIdQueryOutput>.Ok(book.ToGetBookByIdOutput());
        }
    }
}
