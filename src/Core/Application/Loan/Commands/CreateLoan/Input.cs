namespace CreateLoan
{
    public class CreateLoanCommandInput
    {
        public string BookId { get; set; } = string.Empty;
        public string ReaderId { get; set; } = string.Empty;
        public DateOnly? LoanDate { get; set; }
        public DateOnly? DueDate { get; set; }
    }
}
