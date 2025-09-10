using FluentValidation;

namespace CreateLoan
{
    public class CreateLoanCommandValidator : AbstractValidator<CreateLoanCommandInput>
    {
        public CreateLoanCommandValidator()
        {
            RuleFor(x => x.BookId)
                .NotEmpty().WithMessage("BookId is required");

            RuleFor(x => x.ReaderId)
                .NotEmpty().WithMessage("ReaderId is required");

            RuleFor(x => x.LoanDate)
                .Must(d => !d.HasValue || d.Value <= DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("LoanDate cannot be in the future");
        }
    }
}
