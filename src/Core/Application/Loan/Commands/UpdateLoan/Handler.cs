using Application.Interfaces;
using Domain.Models;
using Domain.Common;
using FluentValidation;
using Application.Loans.Mappers;

namespace UpdateLoan
{
    public class UpdateLoanCommandHandler : IUpdateLoanCommandHandler
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IValidator<UpdateLoanCommandInput> _validator;

        public UpdateLoanCommandHandler(ILoanRepository loanRepository, IValidator<UpdateLoanCommandInput> validator)
        {
            _loanRepository = loanRepository ?? throw new ArgumentNullException(nameof(loanRepository));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        public async Task<UpdateLoanCommandOutput?> Handle(UpdateLoanCommandInput input, CancellationToken ct = default)
        {
            await _validator.ValidateAndThrowAsync(input, ct);

            var loanToUpdate = await _loanRepository.GetById(input.Id, ct);
            if (loanToUpdate is null) return null;

            try
            {
                ApplyUpdates(input, loanToUpdate);
            }
            catch (DomainException ex)
            {
                throw new InvalidOperationException(ex.Message, ex);
            }

            await _loanRepository.Update(loanToUpdate, ct);

            return loanToUpdate.ToUpdateLoanOutput();
        }

        private static void ApplyUpdates(UpdateLoanCommandInput input, Loan existing)
        {
            if (!string.IsNullOrWhiteSpace(input.BookId))
                existing.BookId = input.BookId!.Trim();

            if (!string.IsNullOrWhiteSpace(input.ReaderId))
                existing.ReaderId = input.ReaderId!.Trim();

            if (input.LoanDate.HasValue)
                existing.LoanDate = input.LoanDate.Value;

            if (input.DueDate.HasValue)
                existing.DueDate = input.DueDate.Value;

            if (input.ReturnedDate.HasValue)
                existing.ReturnedDate = input.ReturnedDate.Value;

            if (!string.IsNullOrWhiteSpace(input.Status))
            {
                if (Enum.TryParse<LoanStatus>(input.Status, true, out var st))
                    existing.Status = st;
            }
        }
    }
}
