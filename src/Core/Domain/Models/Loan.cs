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
    }
}
