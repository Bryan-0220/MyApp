namespace GetAllLoans
{
    public class GetAllLoansQueryOutput
    {
        public string Id { get; set; } = string.Empty;
        public string BookId { get; set; } = string.Empty;
        public string ReaderId { get; set; } = string.Empty;
        public DateOnly LoanDate { get; set; }
        public DateOnly DueDate { get; set; }
        public DateOnly? ReturnedDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
