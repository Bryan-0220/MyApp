using Application.Interfaces;
using Application.Books.Mappers;

namespace GetAllBooks
{
    public class GetAllBooksQueryHandler : IGetAllBooksQueryHandler
    {
        private readonly IBookRepository _bookRepository;

        public GetAllBooksQueryHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<IEnumerable<GetAllBooksQueryOutput>> Handle(GetAllBooksQueryInput query, CancellationToken ct = default)
        {
            var books = await _bookRepository.GetAll(ct);

            return books.Select(b => b.ToGetAllBooksOutput());
        }
    }
}
