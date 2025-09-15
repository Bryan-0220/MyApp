using Application.Interfaces;
using Application.Books.Services;
using FluentValidation;
using Domain.Common;

namespace DeleteBook
{
    public class DeleteBookCommandHandler : IDeleteBookCommandHandler
    {
        private readonly IBookRepository _bookRepository;
        private readonly IValidator<DeleteBookCommandInput> _validator;
        private readonly IBookService _bookService;

        public DeleteBookCommandHandler(IBookRepository bookRepository, IValidator<DeleteBookCommandInput> validator, IBookService bookService)
        {
            _bookRepository = bookRepository;
            _validator = validator;
            _bookService = bookService;
        }

        public async Task<DeleteBookCommandOutput> HandleAsync(DeleteBookCommandInput input, CancellationToken ct = default)
        {
            await _validator.ValidateAndThrowAsync(input, ct);

            var existing = await _bookRepository.GetByIdAsync(input.Id, ct);
            if (existing is null)
            {
                return new DeleteBookCommandOutput
                {
                    Success = false,
                    Message = "Book not found"
                };
            }

            try
            {
                await _bookService.EnsureCanDelete(input.Id, ct);
            }
            catch (DomainException dex)
            {
                return new DeleteBookCommandOutput
                {
                    Success = false,
                    Message = dex.Message
                };
            }

            await _bookRepository.DeleteAsync(input.Id, ct);
            return new DeleteBookCommandOutput
            {
                Success = true,
                Message = "Book deleted"
            };
        }
    }
}
