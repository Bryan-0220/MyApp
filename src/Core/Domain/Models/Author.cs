using System;
using System.Collections.Generic;
using Domain.Common;

namespace Domain.Models
{
    public class Author
    {
        public string Id { get; set; } = System.Guid.NewGuid().ToString();
        public string Name { get; private set; } = string.Empty;
        public string? Bio { get; private set; }

        // Nueva propiedad: nacionalidad
        public string Nationality { get; private set; } = string.Empty;

        // Fechas de nacimiento y muerte (opcional)
        public DateTime? BirthDate { get; private set; }
        public DateTime? DeathDate { get; private set; }

        // Conjunto de géneros literarios
        public HashSet<string> Genres { get; private set; } = new(StringComparer.OrdinalIgnoreCase);

        public Author() { }

        public static Author Create(string name, string? bio = null, string? nationality = null, DateTime? birthDate = null, DateTime? deathDate = null, IEnumerable<string>? genres = null)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Name is required");
            if (birthDate.HasValue && deathDate.HasValue && deathDate < birthDate) throw new DomainException("DeathDate cannot be before BirthDate");

            var a = new Author
            {
                Name = name.Trim(),
                Bio = string.IsNullOrWhiteSpace(bio) ? null : bio.Trim(),
                Nationality = (nationality ?? string.Empty).Trim(),
                BirthDate = birthDate,
                DeathDate = deathDate
            };

            if (genres != null)
            {
                foreach (var g in genres)
                {
                    if (string.IsNullOrWhiteSpace(g)) continue;
                    a.Genres.Add(g.Trim());
                }
            }

            return a;
        }

        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Name is required");
            Name = name.Trim();
        }

        public void SetBio(string? bio)
        {
            Bio = string.IsNullOrWhiteSpace(bio) ? null : bio.Trim();
        }

        public void SetNationality(string nationality)
        {
            Nationality = (nationality ?? string.Empty).Trim();
        }

        public void SetBirthDate(DateTime? birthDate)
        {
            BirthDate = birthDate;
            if (BirthDate.HasValue && DeathDate.HasValue && DeathDate < BirthDate) throw new DomainException("DeathDate cannot be before BirthDate");
        }

        public void SetDeathDate(DateTime? deathDate)
        {
            DeathDate = deathDate;
            if (BirthDate.HasValue && DeathDate.HasValue && DeathDate < BirthDate) throw new DomainException("DeathDate cannot be before BirthDate");
        }

        public void AddGenre(string genre)
        {
            if (string.IsNullOrWhiteSpace(genre)) throw new DomainException("Genre is required");
            Genres.Add(genre.Trim());
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
                Genres.Add(g.Trim());
            }
        }
    }
}
