namespace Domain.Models
{
    public class User
    {
        public string Id { get; set; } = System.Guid.NewGuid().ToString();
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public System.DateTime MembershipDate { get; set; } = System.DateTime.UtcNow;
    }
}
