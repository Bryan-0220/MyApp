using Application.Interfaces;
using Domain.Models;
using Domain.Common;
using FluentValidation;

namespace UpdateLoan
{
    public class UpdateLoanCommandHandler : IUpdateLoanCommandHandler
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IValidator<UpdateLoanCommandInput> _validator;

        public UpdateLoanCommandHandler(ILoanRepository loanRepository, IValidator<UpdateLoanCommandInput> validator)
        {
            _loanRepository = loanRepository;
            _validator = validator;
        }

        public async Task<UpdateLoanCommandOutput?> HandleAsync(UpdateLoanCommandInput input, CancellationToken ct = default)
        {
            await _validator.ValidateAndThrowAsync(input, ct);

            var existing = await _loanRepository.GetByIdAsync(input.Id, ct);
            if (existing is null) return null;

            try
            {
                applyAttributes(input, existing);
            }
            catch (DomainException ex)
            {
                throw new InvalidOperationException(ex.Message);
            }

            await _loanRepository.UpdateAsync(existing, ct);

            return new UpdateLoanCommandOutput
            {
                Id = existing.Id,
                BookId = existing.BookId,
                ReaderId = existing.ReaderId,
                LoanDate = existing.LoanDate,
                DueDate = existing.DueDate,
                ReturnedDate = existing.ReturnedDate,
                Status = existing.Status.ToString()
            };
        }

        private static void applyAttributes(UpdateLoanCommandInput input, Loan existing)
        {
            if (!string.IsNullOrWhiteSpace(input.BookId) && input.BookId != "string")
                existing.BookId = input.BookId!.Trim();

            if (!string.IsNullOrWhiteSpace(input.ReaderId) && input.ReaderId != "string")
                existing.ReaderId = input.ReaderId!.Trim();

            if (input.LoanDate.HasValue)
                existing.LoanDate = input.LoanDate.Value;

            if (input.DueDate.HasValue)
                existing.DueDate = input.DueDate.Value;

            if (input.ReturnedDate.HasValue)
                existing.ReturnedDate = input.ReturnedDate.Value;

            if (!string.IsNullOrWhiteSpace(input.Status))
            {
                if (Enum.TryParse<LoanStatus>(input.Status, out var st))
                    existing.Status = st;
            }
        }

    }
}
