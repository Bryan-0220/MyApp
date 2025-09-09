namespace FilterAuthors
{
    public class FilterAuthorsQueryInput
    {
        public IEnumerable<string>? Genres { get; set; }
        public string? Name { get; set; }
        public string? Nationality { get; set; }
    }
}
