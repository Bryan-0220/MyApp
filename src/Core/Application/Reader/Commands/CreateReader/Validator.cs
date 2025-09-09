using FluentValidation;
using Application.Interfaces;

namespace CreateReader
{
    public class CreateReaderCommandValidator : AbstractValidator<CreateReaderCommandInput>
    {
        private readonly IReaderRepository _readerRepository;

        public CreateReaderCommandValidator(IReaderRepository readerRepository)
        {
            _readerRepository = readerRepository;

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("FirstName is required")
                .MaximumLength(100).WithMessage("FirstName must be at most 100 characters");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("LastName is required")
                .MaximumLength(100).WithMessage("LastName must be at most 100 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Email must be a valid email address")
                .MaximumLength(320).WithMessage("Email must be at most 320 characters");
        }
    }
}
