using System.Threading;
using System.Threading.Tasks;
using Application.Books.Dtos;
using Application.Interfaces;
using Domain.Models;

namespace Application.Books.Commands.UpdateBook
{
    public class UpdateBookCommandHandler
    {
        private readonly IBookRepository _bookRepository;

        public UpdateBookCommandHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<BookDto?> HandleAsync(UpdateBookCommand command, CancellationToken ct = default)
        {
            var existing = await _bookRepository.GetByIdAsync(command.Id, ct);
            if (existing is null) return null;

            if (!string.IsNullOrEmpty(command.Title)) existing.Title = command.Title;
            if (!string.IsNullOrEmpty(command.AuthorId)) existing.AuthorId = command.AuthorId;
            if (command.ISBN is not null) existing.ISBN = command.ISBN;
            if (command.PublishedYear.HasValue) existing.PublishedYear = command.PublishedYear;
            if (command.CopiesAvailable.HasValue) existing.CopiesAvailable = command.CopiesAvailable.Value;

            await _bookRepository.UpdateAsync(existing, ct);

            return new BookDto
            {
                Id = existing.Id,
                Title = existing.Title,
                AuthorId = existing.AuthorId,
                ISBN = existing.ISBN,
                PublishedYear = existing.PublishedYear,
                CopiesAvailable = existing.CopiesAvailable
            };
        }
    }
}
