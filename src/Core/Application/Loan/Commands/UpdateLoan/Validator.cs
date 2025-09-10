using FluentValidation;

namespace UpdateLoan
{
    public class UpdateLoanCommandValidator : AbstractValidator<UpdateLoanCommandInput>
    {
        public UpdateLoanCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required");

            RuleFor(x => x.LoanDate)
                .Must(d => !d.HasValue || d.Value <= DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("LoanDate cannot be in the future");

            RuleFor(x => x.DueDate)
                .Must(d => !d.HasValue || d.Value >= DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("DueDate cannot be in the past");

            RuleFor(x => x.BookId).Must(s => s == null || s == "string" || !string.IsNullOrWhiteSpace(s));
            RuleFor(x => x.ReaderId).Must(s => s == null || s == "string" || !string.IsNullOrWhiteSpace(s));
        }
    }
}
