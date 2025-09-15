using Application.Interfaces;

namespace GetBookById
{
    public class GetBookByIdQueryHandler : IGetBookByIdQueryHandler
    {
        private readonly IBookRepository _bookRepository;

        public GetBookByIdQueryHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<GetBookByIdQueryOutput?> Handle(GetBookByIdQueryInput query, CancellationToken ct = default)
        {
            var book = await _bookRepository.GetById(query.Id, ct);
            if (book is null) return null;

            return new GetBookByIdQueryOutput
            {
                Id = book.Id,
                Title = book.Title,
                AuthorId = book.AuthorId,
                ISBN = book.ISBN,
                PublishedYear = book.PublishedYear,
                CopiesAvailable = book.CopiesAvailable
                ,
                Genre = book.Genre
            };
        }
    }
}
