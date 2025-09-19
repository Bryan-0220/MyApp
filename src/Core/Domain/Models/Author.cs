using Domain.Common;

namespace Domain.Models
{
    public class Author
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; private set; } = string.Empty;
        public string? Bio { get; private set; }

        public string Nationality { get; private set; } = string.Empty;

        public DateOnly? BirthDate { get; private set; }
        public DateOnly? DeathDate { get; private set; }

        public HashSet<string> Genres { get; private set; } = new(StringComparer.OrdinalIgnoreCase);

        public Author() { }

        public static Author Create(AuthorData input)
        {
            ValidateForCreate(input);

            var author = new Author();

            author.SetName(input.Name!);
            author.SetBio(input.Bio);
            author.SetNationality(input.Nationality!);
            author.SetBirthDate(input.BirthDate);
            author.SetDeathDate(input.DeathDate);

            author.Genres = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var g in input.Genres!)
            {
                if (string.IsNullOrWhiteSpace(g)) continue;
                author.Genres.Add(StringNormalizer.Normalize(g)!);
            }

            return author;
        }

        private static void ValidateForCreate(AuthorData? input)
        {
            if (input == null) throw new BusinessRuleException("Input is required");
            if (string.IsNullOrWhiteSpace(input.Name)) throw new BusinessRuleException("Name is required");
            if (input.Genres == null) throw new BusinessRuleException("Genres are required");

            var hasAnyGenre = input.Genres.Any(g => !string.IsNullOrWhiteSpace(g));
            if (!hasAnyGenre) throw new BusinessRuleException("At least one genre is required");

            if (string.IsNullOrWhiteSpace(input.Nationality)) throw new BusinessRuleException("Nationality is required");

            if (input.BirthDate.HasValue && input.DeathDate.HasValue && input.DeathDate < input.BirthDate)
                throw new BusinessRuleException("DeathDate cannot be before BirthDate");
        }

        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new BusinessRuleException("Name is required");
            Name = StringNormalizer.Normalize(name) ?? string.Empty;
        }

        public void SetBio(string? bio)
        {
            Bio = string.IsNullOrWhiteSpace(bio) ? null : StringNormalizer.Normalize(bio);
        }

        public void SetNationality(string nationality)
        {
            Nationality = StringNormalizer.Normalize(nationality) ?? string.Empty;
        }

        public void SetBirthDate(DateOnly? birthDate)
        {
            BirthDate = birthDate;
            if (BirthDate.HasValue && DeathDate.HasValue && DeathDate < BirthDate) throw new BusinessRuleException("DeathDate cannot be before BirthDate");
        }

        public void SetDeathDate(DateOnly? deathDate)
        {
            DeathDate = deathDate;
            if (BirthDate.HasValue && DeathDate.HasValue && DeathDate < BirthDate) throw new BusinessRuleException("DeathDate cannot be before BirthDate");
        }

        public void AddGenre(string genre)
        {
            if (string.IsNullOrWhiteSpace(genre)) throw new BusinessRuleException("Genre is required");
            Genres.Add(StringNormalizer.Normalize(genre)!);
        }

        public void RemoveGenre(string genre)
        {
            if (string.IsNullOrWhiteSpace(genre)) return;
            Genres.Remove(genre.Trim());
        }

        public void SetGenres(IEnumerable<string>? genres)
        {
            Genres = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (genres == null) return;
            foreach (var g in genres)
            {
                if (string.IsNullOrWhiteSpace(g)) continue;
                Genres.Add(StringNormalizer.Normalize(g)!);
            }
        }
    }
}
