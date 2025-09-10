namespace Application.Filters
{
    public class LoanFilter
    {
        public string? UserId { get; set; }
        public string? BookId { get; set; }
        public bool? Returned { get; set; }
        public DateOnly? LoanDate { get; set; }
        public DateOnly? DueDate { get; set; }
        public DateOnly? ReturnedDate { get; set; }
    }
}
