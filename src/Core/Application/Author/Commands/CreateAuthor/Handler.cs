using Application.Interfaces;
using Application.Authors.Mappers;
using FluentValidation;
using Application.Authors.Services;
using Domain.Models;
using Domain.Common;

namespace CreateAuthor
{
    public class CreateAuthorCommandHandler : ICreateAuthorCommandHandler
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IValidator<CreateAuthorCommandInput> _validator;
        private readonly Application.Authors.Services.IAuthorService _authorService;

        public CreateAuthorCommandHandler(IAuthorRepository authorRepository, IValidator<CreateAuthorCommandInput> validator, Application.Authors.Services.IAuthorService authorService)
        {
            _authorRepository = authorRepository;
            _validator = validator;
            _authorService = authorService;
        }

        public async Task<CreateAuthorCommandOutput> Handle(CreateAuthorCommandInput input, CancellationToken ct = default)
        {
            await _validator.ValidateAndThrowAsync(input, ct);
            await _authorService.EnsureCanCreate(input.Name, ct);

            var author = Author.Create(input.ToData());
            var created = await _authorRepository.Create(author, ct);

            return created.ToOutput();
        }
    }
}
