namespace Application.Filters
{
    public class ReaderFilter
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public DateOnly? FromMembershipDate { get; set; }
        public DateOnly? ToMembershipDate { get; set; }
    }
}
