namespace Application.Filters
{
    public class LoanFilter
    {
        public string? UserId { get; set; }
        public string? BookId { get; set; }
        public bool? Returned { get; set; }
        public System.DateTime? FromDate { get; set; }
        public System.DateTime? ToDate { get; set; }
    }
}
