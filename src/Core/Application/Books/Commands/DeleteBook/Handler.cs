using Application.Interfaces;
using Application.Books.Services;
using FluentValidation;
using Domain.Common;
using Application.Books.Mappers;

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

        public async Task<DeleteBookCommandOutput> Handle(DeleteBookCommandInput input, CancellationToken ct = default)
        {
            await _validator.ValidateAndThrowAsync(input, ct);

            var existing = await _bookRepository.GetById(input.Id, ct);
            if (existing is null)
            {
                return (null as Domain.Models.Book).ToDeleteBookOutput(false, "Book not found");
            }

            try
            {
                await _bookService.EnsureCanDelete(input.Id, ct);
            }
            catch (DomainException dex)
            {
                return existing.ToDeleteBookOutput(false, dex.Message);
            }

            await _bookRepository.Delete(input.Id, ct);
            return existing.ToDeleteBookOutput(true, "Book deleted");
        }
    }
}
