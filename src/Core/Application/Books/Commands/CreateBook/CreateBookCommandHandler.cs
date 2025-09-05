using Domain.Models;
using Application.Books.Dtos;
using Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Books.Commands.CreateBook
{
    public class CreateBookCommandHandler
    {
        private readonly IBookRepository _bookRepository;

        public CreateBookCommandHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<BookDto> HandleAsync(CreateBookCommand command, CancellationToken ct = default)
        {
            var book = new Book
            {
                Title = command.Title,
                AuthorId = command.AuthorId,
                ISBN = command.ISBN,
                PublishedYear = command.PublishedYear,
                CopiesAvailable = command.CopiesAvailable
            };

            var created = await _bookRepository.CreateAsync(book, ct);

            return new BookDto
            {
                Id = created.Id,
                Title = created.Title,
                AuthorId = created.AuthorId,
                ISBN = created.ISBN,
                PublishedYear = created.PublishedYear,
                CopiesAvailable = created.CopiesAvailable
            };
        }
    }
}
