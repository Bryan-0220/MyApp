using Application.Filters;
using Application.Interfaces;
using Domain.Common;

namespace Application.Authors.Services
{
    public class AuthorCreationService : IAuthorCreationService
    {
        private readonly IAuthorRepository _authorRepository;

        public AuthorCreationService(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        public async Task EnsureCanCreateAsync(string name, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Name is required");

            var filter = new AuthorFilter
            {
                Name = name.Trim()
            };

            var existing = await _authorRepository.FilterAsync(filter, ct);
            if (existing != null && existing.Any())
            {
                throw new DomainException("An author with the same name already exists.");
            }
        }
    }
}
