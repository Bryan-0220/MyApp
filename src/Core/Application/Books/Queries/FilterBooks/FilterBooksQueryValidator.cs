using FluentValidation;

namespace FilterBooks
{
    public class FilterBooksQueryValidator : AbstractValidator<FilterBooksQueryInput>
    {
        public FilterBooksQueryValidator()
        {
            // Genre is optional, but if present should not be empty whitespace
            RuleFor(x => x.Genre).Must(g => string.IsNullOrWhiteSpace(g) == false).When(x => x.Genre != null)
                .WithMessage("Genre must not be empty if provided.");
        }
    }
}
