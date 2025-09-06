using Application.Interfaces;
using FluentValidation;

namespace DeleteBook
{
    public class DeleteBookCommandHandler : IDeleteBookCommandHandler
    {
        private readonly IBookRepository _bookRepository;
        private readonly FluentValidation.IValidator<DeleteBookCommandInput> _validator;

        public DeleteBookCommandHandler(IBookRepository bookRepository, FluentValidation.IValidator<DeleteBookCommandInput> validator)
        {
            _bookRepository = bookRepository;
            _validator = validator;
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

            await _bookRepository.DeleteAsync(input.Id, ct);
            return new DeleteBookCommandOutput
            {
                Success = true,
                Message = "Book deleted"
            };
        }
    }
}
