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
        private readonly IAuthorService _authorService;

        public CreateAuthorCommandHandler(IAuthorRepository authorRepository, IValidator<CreateAuthorCommandInput> validator, Application.Authors.Services.IAuthorService authorService)
        {
            _authorRepository = authorRepository;
            _validator = validator;
            _authorService = authorService;
        }

        public async Task<CreateAuthorCommandOutput> Handle(CreateAuthorCommandInput input, CancellationToken ct = default)
        {
            await _validator.ValidateAndThrowAsync(input, ct);
            var created = await _authorService.CreateAuthor(input.ToData(), ct);
            return created.ToCreateAuthorOutput();
        }

    }
}
