using Domain.Models;
using Domain.Common;
using Application.Interfaces;
using FluentValidation;

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
                book = Book.Create(input.Title, input.AuthorId, input.ISBN, input.PublishedYear, input.CopiesAvailable);
            }
            catch (DomainException ex)
            {
                throw new InvalidOperationException(ex.Message);
            }

            var created = await _bookRepository.CreateAsync(book, ct);

            return new CreateBookCommandOutput
            {
                Id = created.Id,
                Title = created.Title,
                AuthorId = created.AuthorId,
                ISBN = created.ISBN,
                PublishedYear = created.PublishedYear,
                CopiesAvailable = created.CopiesAvailable
            };
        }
    }
}
