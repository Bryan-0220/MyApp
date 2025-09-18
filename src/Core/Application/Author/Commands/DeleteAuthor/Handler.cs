using Application.Interfaces;
using Application.Authors.Mappers;
using FluentValidation;
using Application.Authors.Services;
using Domain.Common;

namespace DeleteAuthor
{
    public class DeleteAuthorCommandHandler : IDeleteAuthorCommandHandler
    {
        private readonly IAuthorService _deletionService;
        private readonly IValidator<DeleteAuthorCommandInput> _validator;

        public DeleteAuthorCommandHandler(IAuthorService deletionService, IValidator<DeleteAuthorCommandInput> validator)
        {
            _deletionService = deletionService;
            _validator = validator;
        }

        public async Task<DeleteAuthorCommandOutput> Handle(DeleteAuthorCommandInput input, CancellationToken ct = default)
        {
            await _validator.ValidateAndThrowAsync(input, ct);
            var result = await _deletionService.DeleteAuthor(input.Id, ct);
            return result.ToDeleteAuthorOutput();
        }
    }
}
