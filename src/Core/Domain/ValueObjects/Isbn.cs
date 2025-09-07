namespace Domain.ValueObjects
{
    public sealed class Isbn
    {
        public string Value { get; }

        private Isbn(string value)
        {
            Value = value;
        }

        public static bool TryParse(string? input, out Isbn? isbn, out string? error)
        {
            isbn = null;
            error = null;

            if (string.IsNullOrWhiteSpace(input))
            {
                error = "ISBN is empty";
                return false;
            }

            var normalized = input.Trim();

            if (normalized.Length != 5)
            {
                error = "ISBN must be exactly 5 characters";
                return false;
            }

            isbn = new Isbn(normalized);
            return true;
        }

        public override string ToString() => Value;
    }
}
