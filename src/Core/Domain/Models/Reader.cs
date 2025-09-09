namespace Domain.Models
{
    public class Reader
    {
        public string Id { get; set; } = System.Guid.NewGuid().ToString();
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime MembershipDate { get; set; } = DateTime.UtcNow;
    }
}
