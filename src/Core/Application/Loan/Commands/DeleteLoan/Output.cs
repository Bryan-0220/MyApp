namespace DeleteLoan
{
    public class DeleteLoanCommandOutput
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? LoanId { get; set; }
    }
}
