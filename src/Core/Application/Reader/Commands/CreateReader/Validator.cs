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

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("FullName is required")
                .MaximumLength(200).WithMessage("FullName must be at most 200 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Email must be a valid email address")
                .MaximumLength(320).WithMessage("Email must be at most 320 characters");
        }
    }
}
