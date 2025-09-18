using Domain.Models;
using Domain.Results;

namespace Application.Loans.Services
{
    public interface ILoanService
    {
        Task EnsureNoDuplicateLoan(string bookId, string readerId, CancellationToken ct = default);
        Task<Loan> CreateLoan(LoanData data, CancellationToken ct = default);
        Task<Loan> UpdateLoan(UpdateLoan.UpdateLoanCommandInput input, CancellationToken ct = default);

        Task<Result<Loan>> DeleteLoan(string id, CancellationToken ct = default);
    }
}
