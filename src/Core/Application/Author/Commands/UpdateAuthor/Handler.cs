using Application.Authors.Mappers;
using Application.Authors.Services;
using FluentValidation;

namespace UpdateAuthor
{
    public class UpdateAuthorCommandHandler : IUpdateAuthorCommandHandler
    {
        private readonly IAuthorService _authorService;
        private readonly IValidator<UpdateAuthorCommandInput> _validator;

        public UpdateAuthorCommandHandler(IAuthorService authorService, IValidator<UpdateAuthorCommandInput> validator)
        {
            _authorService = authorService;
            _validator = validator;
        }

        public async Task<UpdateAuthorCommandOutput> Handle(UpdateAuthorCommandInput input, CancellationToken ct = default)
        {
            await _validator.ValidateAndThrowAsync(input, ct);
            var updated = await _authorService.UpdateAuthor(input, ct);
            return updated.ToUpdateAuthorOutput();
        }
    }
}
