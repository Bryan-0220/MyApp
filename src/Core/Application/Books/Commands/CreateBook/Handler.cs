using Domain.Models;
using Domain.Common;
using Application.Interfaces;
using FluentValidation;
using Application.Books.Mappers;

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

        public async Task<CreateBookCommandOutput> HandleAsync(CreateBookCommandInput input, CancellationToken ct = default)
        {
            await _validator.ValidateAndThrowAsync(input, ct);

            Book book;
            try
            {
                book = Book.Create(input.ToData());
            }
            catch (DomainException ex)
            {
                throw new InvalidOperationException(ex.Message);
            }

            var created = await _bookRepository.CreateAsync(book, ct);

            return created.ToCreateBookOutput();
        }
    }
}
