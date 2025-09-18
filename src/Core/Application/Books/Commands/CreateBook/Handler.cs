using Application.Interfaces;
using Application.Books.Mappers;
using FluentValidation;
using Domain.Models;

namespace CreateBook
{
    public class CreateBookCommandHandler : ICreateBookCommandHandler
    {
        private readonly IBookRepository _bookRepository;
        private readonly IValidator<CreateBookCommandInput> _validator;

        public CreateBookCommandHandler(IBookRepository bookRepository, IValidator<CreateBookCommandInput> validator)
        {
            _bookRepository = bookRepository;
            _validator = validator;
        }

        public async Task<CreateBookCommandOutput> Handle(CreateBookCommandInput input, CancellationToken ct = default)
        {
            await _validator.ValidateAndThrowAsync(input, ct);
            var book = Book.Create(input.ToData());
            var created = await _bookRepository.Create(book, ct);
            return created.ToCreateBookOutput();
        }
    }
}
