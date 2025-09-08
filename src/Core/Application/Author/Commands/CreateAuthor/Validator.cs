using FluentValidation;
using Application.Interfaces;

namespace CreateAuthor
{
    public class CreateAuthorCommandValidator : AbstractValidator<CreateAuthorCommandInput>
    {
        private readonly IAuthorRepository _authorRepository;

        public CreateAuthorCommandValidator(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(200).WithMessage("Name must be at most 200 characters");

            RuleFor(x => x)
                .Must(x =>
                {
                    if (!x.BirthDate.HasValue || !x.DeathDate.HasValue) return true;
                    return x.BirthDate.Value <= x.DeathDate.Value;
                })
                .WithMessage("BirthDate must be before or equal to DeathDate");

            RuleForEach(x => x.Genres)
                .NotEmpty().WithMessage("Genres cannot contain empty values")
                .MaximumLength(100).WithMessage("Each genre must be at most 100 characters");

            RuleFor(x => x.Nationality)
                .MaximumLength(100).WithMessage("Nationality must be at most 100 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.Nationality));
        }
    }
}
