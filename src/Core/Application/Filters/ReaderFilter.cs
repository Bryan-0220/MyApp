namespace Application.Filters
{
    public class ReaderFilter
    {
        public string? FirstName { get; init; }
        public string? LastName { get; init; }
        public string? Email { get; init; }
        public DateOnly? FromMembershipDate { get; init; }
        public DateOnly? ToMembershipDate { get; init; }

        public ReaderFilter() { }

        private ReaderFilter(string? firstName, string? lastName, string? email, DateOnly? fromMembershipDate, DateOnly? toMembershipDate)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            FromMembershipDate = fromMembershipDate;
            ToMembershipDate = toMembershipDate;
        }

        public static ReaderFilter Create(string? firstName = null, string? lastName = null, string? email = null, DateOnly? fromMembershipDate = null, DateOnly? toMembershipDate = null)
        {
            var cleanedFirst = string.IsNullOrWhiteSpace(firstName) ? null : firstName.Trim();
            var cleanedLast = string.IsNullOrWhiteSpace(lastName) ? null : lastName.Trim();
            var cleanedEmail = string.IsNullOrWhiteSpace(email) ? null : email.Trim();

            return new ReaderFilter(cleanedFirst, cleanedLast, cleanedEmail, fromMembershipDate, toMembershipDate);
        }
    }
}
