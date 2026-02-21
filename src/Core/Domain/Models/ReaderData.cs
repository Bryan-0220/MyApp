namespace Domain.Models
{
    public class ReaderData
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateOnly? MembershipDate { get; set; }
    }
}
