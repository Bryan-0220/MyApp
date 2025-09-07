using Application.Interfaces;

namespace GetAllBooks
{
    public class GetAllBooksQueryHandler : IGetAllBooksQueryHandler
    {
        private readonly IBookRepository _bookRepository;

        public GetAllBooksQueryHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<IEnumerable<GetAllBooksQueryOutput>> HandleAsync(GetAllBooksQueryInput query, CancellationToken ct = default)
        {
            var books = await _bookRepository.GetAllAsync(ct);

            return books.Select(b => new GetAllBooksQueryOutput
            {
                Id = b.Id,
                Title = b.Title,
                AuthorId = b.AuthorId,
                ISBN = b.ISBN,
                PublishedYear = b.PublishedYear,
                CopiesAvailable = b.CopiesAvailable
                ,
                Genre = b.Genre
            });
        }
    }
}
