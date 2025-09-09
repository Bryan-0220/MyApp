using FluentValidation;
using Application.Interfaces;

namespace UpdateReader
{
    public class UpdateReaderCommandValidator : AbstractValidator<UpdateReaderCommandInput>
    {
        private readonly IReaderRepository _readerRepository;

        public UpdateReaderCommandValidator(IReaderRepository readerRepository)
        {
            _readerRepository = readerRepository;

            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required");

            RuleFor(x => x.FirstName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("FirstName is required")
                .MaximumLength(100).WithMessage("FirstName must be at most 100 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.FirstName) && x.FirstName != "string");

            RuleFor(x => x.LastName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("LastName is required")
                .MaximumLength(100).WithMessage("LastName must be at most 100 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.LastName) && x.LastName != "string");

            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .EmailAddress().WithMessage("Email must be a valid email address")
                .MaximumLength(320).WithMessage("Email must be at most 320 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.Email) && x.Email != "string");

            RuleFor(x => x.MembershipDate)
                .Must(d => !d.HasValue || d.Value <= DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("MembershipDate cannot be in the future");
        }
    }
}
