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
        public System.DateTime LoanDate { get; set; } = System.DateTime.UtcNow;
        public System.DateTime DueDate { get; set; } = System.DateTime.UtcNow.AddDays(14);
        public System.DateTime? ReturnedDate { get; set; }
        public LoanStatus Status { get; set; } = LoanStatus.Active;
    }
}
