using FluentValidation;
using Application.Interfaces;

namespace UpdateBook
{
    public class UpdateBookCommandValidator : AbstractValidator<UpdateBookCommandInput>
    {
        private readonly IBookRepository _bookRepository;

        public UpdateBookCommandValidator(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;

            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required");

            RuleFor(x => x.Title)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(200).WithMessage("Title must be at most 200 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.Title) && x.Title != "string");

            RuleFor(x => x.AuthorId)
                .NotEmpty().WithMessage("AuthorId is required")
                .When(x => !string.IsNullOrWhiteSpace(x.AuthorId) && x.AuthorId != "string");

            RuleFor(x => x.ISBN)
                .Cascade(CascadeMode.Stop)
                .Must(isbn => string.IsNullOrWhiteSpace(isbn) || isbn == "string" || isbn.Trim().Length == 5)
                .WithMessage("ISBN must be exactly 5 characters")
                .MustAsync(async (input, isbn, ct) =>
                {
                    if (string.IsNullOrWhiteSpace(isbn) || isbn == "string") return true;
                    var count = await _bookRepository.Count(b => b.ISBN == isbn && b.Id != input.Id, ct);
                    return count == 0;
                })
                .WithMessage("Ya existe un libro con ese ISBN.");

            RuleFor(x => x.PublishedYear)
                .InclusiveBetween(0, 9999).WithMessage("PublishedYear must be a valid year")
                .When(x => x.PublishedYear.HasValue && x.PublishedYear.Value != -1);

            RuleFor(x => x.CopiesAvailable)
                .GreaterThanOrEqualTo(0).WithMessage("CopiesAvailable must be >= 0")
                .When(x => x.CopiesAvailable.HasValue && x.CopiesAvailable.Value != -1);
        }
    }
}
