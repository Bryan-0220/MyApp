namespace UpdateLoan
{
    public class UpdateLoanCommandInput
    {
        public string Id { get; set; } = string.Empty;
        public string? BookId { get; set; }
        public string? ReaderId { get; set; }
        public DateOnly? LoanDate { get; set; }
        public DateOnly? DueDate { get; set; }
        public DateOnly? ReturnedDate { get; set; }
        public string? Status { get; set; }
    }
}
