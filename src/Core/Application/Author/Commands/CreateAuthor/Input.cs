namespace CreateAuthor
{
    public class CreateAuthorCommandInput
    {
        public string Name { get; set; } = string.Empty;
        public string? Bio { get; set; }

        // Nueva propiedades
        public string? Nationality { get; set; }
        public System.DateTime? BirthDate { get; set; }
        public System.DateTime? DeathDate { get; set; }
        public System.Collections.Generic.IEnumerable<string>? Genres { get; set; }
    }
}
