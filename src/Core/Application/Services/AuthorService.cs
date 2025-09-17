using Application.Filters;
using Application.Interfaces;
using Domain.Common;
using Domain.Models;
using Domain.Results;

namespace Application.Authors.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IBookRepository _bookRepository;

        public AuthorService(IAuthorRepository authorRepository, IBookRepository bookRepository)
        {
            _authorRepository = authorRepository;
            _bookRepository = bookRepository;
        }

        public async Task EnsureCanCreate(string name, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Name is required");

            var filter = new AuthorFilter
            {
                Name = name.Trim()
            };

            var existing = await _authorRepository.Filter(filter, ct);
            if (existing != null && existing.Any())
                throw new DomainException("An author with the same name already exists.");
        }

        public async Task EnsureCanDelete(string authorId, CancellationToken ct = default)
        {
            var filter = new BookFilter
            {
                AuthorId = authorId
            };

            var books = await _bookRepository.Filter(filter, ct);
            if (books != null && books.Any())
            {
                throw new DomainException("Author cannot be deleted while they have registered books.");
            }
        }

        public async Task<Result<Author>> DeleteAuthor(string authorId, CancellationToken ct = default)
        {
            var existing = await _authorRepository.GetById(authorId, ct);
            if (existing is null)
                return Domain.Results.Result<Domain.Models.Author>.Fail("Author not found");

            try
            {
                await EnsureCanDelete(authorId, ct);
            }
            catch (DomainException dex)
            {
                return Domain.Results.Result<Domain.Models.Author>.Fail(dex.Message);
            }

            await _authorRepository.Delete(authorId, ct);
            return Domain.Results.Result<Domain.Models.Author>.Ok(existing, "Author deleted");
        }
    }
}
