using FluentValidation;

namespace FilterAuthors
{
    public class FilterAuthorsQueryValidator : AbstractValidator<FilterAuthorsQueryInput>
    {
        public FilterAuthorsQueryValidator()
        {
            RuleFor(x => x.Name).Must(n => string.IsNullOrWhiteSpace(n) == false).When(x => x.Name != null)
                .WithMessage("Name must not be empty if provided.");

            RuleFor(x => x.Nationality).Must(n => string.IsNullOrWhiteSpace(n) == false).When(x => x.Nationality != null)
                .WithMessage("Nationality must not be empty if provided.");

            RuleForEach(x => x.Genres).NotEmpty().WithMessage("Genres cannot contain empty values");
        }
    }
}
