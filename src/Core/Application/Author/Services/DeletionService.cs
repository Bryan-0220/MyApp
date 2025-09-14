using Application.Interfaces;
using Application.Filters;
using Domain.Common;

namespace Application.Authors.Services
{
    public class AuthorDeletionService : IAuthorDeletionService
    {
        private readonly IBookRepository _bookRepository;

        public AuthorDeletionService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task EnsureCanDeleteAsync(string authorId, CancellationToken ct = default)
        {
            var filter = new BookFilter
            {
                AuthorId = authorId
            };

            var books = await _bookRepository.FilterAsync(filter, ct);
            if (books != null && books.Any())
            {
                throw new DomainException("Author cannot be deleted while they have registered books.");
            }
        }
    }
}
