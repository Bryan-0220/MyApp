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
        public string Id { get; set; } = System.Guid.NewGuid().ToString();
        public string BookId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        // public System.DateOnly LoanDate { get; set; } = System.DateOnly.UtcNow;
        // public System.DateOnly DueDate { get; set; } = System.DateOnly.UtcNow.AddDays(14);
        public System.DateOnly? ReturnedDate { get; set; }
        public LoanStatus Status { get; set; } = LoanStatus.Active;
    }
}
