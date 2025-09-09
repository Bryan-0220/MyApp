namespace UpdateReader
{
    public class UpdateReaderCommandInput
    {
        public string Id { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public DateOnly? MembershipDate { get; set; }
    }
}
