using FluentValidation;
using Application.Interfaces;

namespace UpdateAuthor
{
    public class UpdateAuthorCommandValidator : AbstractValidator<UpdateAuthorCommandInput>
    {
        private readonly IAuthorRepository _authorRepository;

        public UpdateAuthorCommandValidator(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;

            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(200).WithMessage("Name must be at most 200 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.Name) && x.Name != "string");

            RuleFor(x => x.Nationality)
                .MaximumLength(100).WithMessage("Nationality must be at most 100 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.Nationality) && x.Nationality != "string");

            RuleFor(x => x.BirthDate)
                .Must(d => !d.HasValue || d.Value != default(DateTime)).WithMessage("BirthDate is not valid")
                .When(x => x.BirthDate.HasValue)
                .LessThanOrEqualTo(x => x.DeathDate ?? x.BirthDate)
                .When(x => x.BirthDate.HasValue && x.DeathDate.HasValue)
                .WithMessage("BirthDate must be before or equal to DeathDate");

            RuleFor(x => x.DeathDate)
                .Must(d => !d.HasValue || d.Value != default(DateTime)).WithMessage("DeathDate is not valid")
                .When(x => x.DeathDate.HasValue);

            RuleForEach(x => x.Genres)
                .NotEmpty().WithMessage("Genres cannot contain empty values")
                .When(x => x.Genres != null && x.Genres.Any(g => !string.IsNullOrWhiteSpace(g) && g.Trim() != "string"));
        }
    }
}
