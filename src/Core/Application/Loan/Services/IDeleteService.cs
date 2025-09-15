using Domain.Models;

namespace Application.Loans.Services
{
    public interface IDeleteService
    {
        Task<bool> EnsureCanDeleteAsync(Loan loan, CancellationToken ct = default);
        Task HandlePostDeleteAsync(Loan loan, CancellationToken ct = default);
    }
}
