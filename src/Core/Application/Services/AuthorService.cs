using Application.Filters;
using Application.Interfaces;
using Domain.Common;
using Domain.Models;
using Domain.Results;
using UpdateAuthor;

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
                throw new DomainException("Author cannot be deleted while they have registered books.");
        }

        public async Task<Author> CreateAuthor(AuthorData input, CancellationToken ct)
        {
            await EnsureCanCreate(input.Name, ct);
            var author = Author.Create(input);
            var created = await _authorRepository.Create(author, ct);
            return created;
        }

        public async Task<Author> UpdateAuthor(UpdateAuthorCommandInput input, CancellationToken ct = default)
        {
            var existing = await _authorRepository.GetById(input.Id, ct);
            if (existing is null)
                throw new DomainException("Author not found");

            ApplyAttributes(input, existing);

            await _authorRepository.Update(existing, ct);
            return existing;
        }

        private static void ApplyAttributes(UpdateAuthorCommandInput input, Author existing)
        {
            if (!string.IsNullOrWhiteSpace(input.Name) && input.Name != "string")
                existing.SetName(input.Name!.Trim());

            if (!string.IsNullOrWhiteSpace(input.Bio) && input.Bio != "string")
                existing.SetBio(input.Bio!.Trim());

            if (!string.IsNullOrWhiteSpace(input.Nationality) && input.Nationality != "string")
                existing.SetNationality(input.Nationality!.Trim());

            if (input.BirthDate.HasValue)
                existing.SetBirthDate(input.BirthDate);

            if (input.DeathDate.HasValue)
                existing.SetDeathDate(input.DeathDate);

            if (input.Genres != null)
                existing.SetGenres(input.Genres);
        }

        public async Task<Result<Author>> DeleteAuthor(string authorId, CancellationToken ct = default)
        {
            var existing = await _authorRepository.GetById(authorId, ct);
            if (existing is null)
                return Result<Author>.Fail("Author not found");
            try
            {
                await EnsureCanDelete(authorId, ct);
            }
            catch (DomainException dex)
            {
                return Result<Author>.Fail(dex.Message);
            }

            await _authorRepository.Delete(authorId, ct);
            return Result<Author>.Ok(existing, "Author deleted");
        }
    }
}
