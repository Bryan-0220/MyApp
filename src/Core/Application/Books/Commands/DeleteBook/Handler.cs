using Application.Interfaces;
using Application.Books.Services;
using FluentValidation;
using Domain.Common;
using Application.Books.Mappers;

namespace DeleteBook
{
    public class DeleteBookCommandHandler : IDeleteBookCommandHandler
    {
        private readonly IValidator<DeleteBookCommandInput> _validator;
        private readonly IBookService _bookService;

        public DeleteBookCommandHandler(IValidator<DeleteBookCommandInput> validator, IBookService bookService)
        {
            _validator = validator;
            _bookService = bookService;
        }

        public async Task<DeleteBookCommandOutput> Handle(DeleteBookCommandInput input, CancellationToken ct = default)
        {
            await _validator.ValidateAndThrowAsync(input, ct);
            var result = await _bookService.DeleteBook(input.Id, ct);
            return result.ToDeleteBookOutput();
        }
    }
}
