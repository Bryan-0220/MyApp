using Domain.Common;

namespace Domain.Models
{
    public enum LoanStatus
    {
        Active,
        Returned,
        Overdue
    }

    public class Loan
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string BookId { get; set; } = string.Empty;
        public string ReaderId { get; set; } = string.Empty;
        public DateOnly LoanDate { get; set; }
        public DateOnly DueDate { get; set; }
        public DateOnly? ReturnedDate { get; set; }
        public LoanStatus Status { get; set; } = LoanStatus.Active;

        public static Loan Create(LoanData data)
        {
            if (data == null) throw new DomainException("Input is required");

            var bookId = (data.BookId ?? string.Empty).Trim();
            var readerId = (data.ReaderId ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(bookId) || string.IsNullOrWhiteSpace(readerId) || data.LoanDate == null || data.DueDate == null)
            {
                throw new DomainException("All loan attributes (BookId, ReaderId, LoanDate, DueDate) are required");
            }

            var loanDate = data.LoanDate.Value;
            var dueDate = data.DueDate.Value;

            if (dueDate < loanDate)
            {
                throw new DomainException("DueDate must be the same day as LoanDate or later");
            }

            var loan = new Loan
            {
                BookId = bookId,
                ReaderId = readerId,
                LoanDate = loanDate,
                DueDate = dueDate
            };

            return loan;
        }

        public bool CanBeDeleted(out string? reason)
        {
            if (this.Status != LoanStatus.Returned)
            {
                reason = "Cannot delete an active or overdue loan. Mark it as returned before deletion.";
                return false;
            }
            reason = null;
            return true;
        }
    }
}
