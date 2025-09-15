using Domain.Models;
using GetAllLoans;
using GetLoanById;
using FilterLoans;
using CreateLoan;
using UpdateLoan;
using DeleteLoan;

namespace Application.Loans.Mappers
{
    public static class LoanMapper
    {
        public static Domain.Models.LoanData ToData(this CreateLoan.CreateLoanCommandInput input)
        {
            if (input == null) return null!;

            return new Domain.Models.LoanData
            {
                BookId = input.BookId?.Trim() ?? string.Empty,
                ReaderId = input.ReaderId?.Trim() ?? string.Empty,
                LoanDate = input.LoanDate,
                DueDate = input.DueDate
            };
        }
        public static GetAllLoansQueryOutput ToGetAllLoansOutput(this Loan loan)
        {
            return new GetAllLoansQueryOutput
            {
                Id = loan.Id,
                BookId = loan.BookId,
                ReaderId = loan.ReaderId,
                LoanDate = loan.LoanDate,
                DueDate = loan.DueDate,
                ReturnedDate = loan.ReturnedDate,
                Status = loan.Status.ToString()
            };
        }

        public static GetLoanByIdQueryOutput ToGetLoanByIdOutput(this Loan loan)
        {
            return new GetLoanByIdQueryOutput
            {
                Id = loan.Id,
                BookId = loan.BookId,
                ReaderId = loan.ReaderId,
                LoanDate = loan.LoanDate,
                DueDate = loan.DueDate,
                ReturnedDate = loan.ReturnedDate,
                Status = loan.Status.ToString()
            };
        }

        public static FilterLoansQueryOutput ToFilterLoansOutput(this Loan loan)
        {
            return new FilterLoansQueryOutput
            {
                Id = loan.Id,
                BookId = loan.BookId,
                ReaderId = loan.ReaderId,
                LoanDate = loan.LoanDate,
                DueDate = loan.DueDate,
                ReturnedDate = loan.ReturnedDate,
                Status = loan.Status.ToString()
            };
        }

        public static CreateLoanCommandOutput ToCreateLoanOutput(this Loan loan)
        {
            return new CreateLoanCommandOutput
            {
                Id = loan.Id,
                BookId = loan.BookId,
                ReaderId = loan.ReaderId,
                LoanDate = loan.LoanDate,
                DueDate = loan.DueDate,
                ReturnedDate = loan.ReturnedDate,
                Status = loan.Status.ToString()
            };
        }

        public static UpdateLoanCommandOutput ToUpdateLoanOutput(this Loan loan)
        {
            return new UpdateLoanCommandOutput
            {
                Id = loan.Id,
                BookId = loan.BookId,
                ReaderId = loan.ReaderId,
                LoanDate = loan.LoanDate,
                DueDate = loan.DueDate,
                ReturnedDate = loan.ReturnedDate,
                Status = loan.Status.ToString()
            };
        }

        public static DeleteLoanCommandOutput ToDeleteLoanOutput(this Loan? loan, bool deleted, string? message = null)
        {
            return new DeleteLoanCommandOutput
            {
                Deleted = deleted,
                Message = message
            };
        }
    }
}
