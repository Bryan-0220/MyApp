using Application.Books.Mappers;
using Application.Books.Services;
using FluentValidation;

namespace UpdateBook
{
    public class UpdateBookCommandHandler : IUpdateBookCommandHandler
    {
        private readonly IBookService _bookService;
        private readonly IValidator<UpdateBookCommandInput> _validator;

        public UpdateBookCommandHandler(IBookService bookService, IValidator<UpdateBookCommandInput> validator)
        {
            _bookService = bookService;
            _validator = validator;
        }

        public async Task<UpdateBookCommandOutput> Handle(UpdateBookCommandInput input, CancellationToken ct = default)
        {
            await _validator.ValidateAndThrowAsync(input, ct);
            var updated = await _bookService.UpdateBook(input, ct);
            return updated.ToUpdateBookOutput();
        }
    }
}
