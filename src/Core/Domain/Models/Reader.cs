using Domain.Common;

namespace Domain.Models
{
    public class Reader
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateOnly MembershipDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

        public static Reader Create(ReaderData data)
        {
            if (data == null) throw new DomainException("Input is required");

            if (string.IsNullOrWhiteSpace(data.FirstName)) throw new DomainException("FirstName is required");
            if (string.IsNullOrWhiteSpace(data.LastName)) throw new DomainException("LastName is required");
            if (string.IsNullOrWhiteSpace(data.Email)) throw new DomainException("Email is required");

            return new Reader
            {
                FirstName = data.FirstName.Trim(),
                LastName = data.LastName.Trim(),
                Email = data.Email.Trim(),
                MembershipDate = data.MembershipDate ?? DateOnly.FromDateTime(DateTime.UtcNow)
            };
        }
    }
}
