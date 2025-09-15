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
        public DateOnly LoanDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        public DateOnly DueDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(14);
        public DateOnly? ReturnedDate { get; set; }
        public LoanStatus Status { get; set; } = LoanStatus.Active;

        public static Loan Create(LoanData data)
        {
            if (data == null) throw new Domain.Common.DomainException("Input is required");

            var now = DateOnly.FromDateTime(DateTime.UtcNow);

            var loan = new Loan
            {
                BookId = (data.BookId ?? string.Empty).Trim(),
                ReaderId = (data.ReaderId ?? string.Empty).Trim(),
                LoanDate = data.LoanDate ?? now,
                DueDate = data.DueDate ?? now.AddDays(14)
            };

            if (string.IsNullOrWhiteSpace(loan.BookId)) throw new Domain.Common.DomainException("BookId is required");
            if (string.IsNullOrWhiteSpace(loan.ReaderId)) throw new Domain.Common.DomainException("ReaderId is required");

            return loan;
        }
    }
}
