using Domain.Models;
using Domain.Results;

namespace Application.Loans.Services
{
    public interface ILoanService
    {
        Task EnsureNoDuplicateLoan(string bookId, string readerId, CancellationToken ct = default);

        Task<Result<Loan>> DeleteLoanById(string id, CancellationToken ct = default);
    }
}
