namespace Application.Filters
{
    public class LoanFilter
    {
        public string? UserId { get; set; }
        public string? BookId { get; set; }
        public bool? Returned { get; set; }
        public System.DateOnly? FromDate { get; set; }
        public System.DateOnly? ToDate { get; set; }
    }
}
