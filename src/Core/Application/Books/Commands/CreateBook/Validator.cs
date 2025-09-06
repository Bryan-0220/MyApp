using FluentValidation;
using Application.Interfaces;

namespace CreateBook
{
    public class CreateBookCommandValidator : AbstractValidator<CreateBookCommandInput>
    {
        private readonly IBookRepository _bookRepository;

        public CreateBookCommandValidator(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(200).WithMessage("Title must be at most 200 characters");

            RuleFor(x => x.AuthorId)
                .NotEmpty().WithMessage("AuthorId is required");

            RuleFor(x => x.ISBN)
                .NotEmpty()
                .MustAsync(async (isbn, ct) =>
                {
                    var exists = await _bookRepository.CountAsync(b => b.ISBN == isbn, ct);
                    return exists == 0;
                })
                .WithMessage("Ya existe un libro con ese ISBN.");

            RuleFor(x => x.PublishedYear)
                .InclusiveBetween(0, 9999).WithMessage("PublishedYear must be a valid year")
                .When(x => x.PublishedYear.HasValue);

            RuleFor(x => x.CopiesAvailable)
                .GreaterThan(0).WithMessage("CopiesAvailable must be > 0");
        }
    }
}
