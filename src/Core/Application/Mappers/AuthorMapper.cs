using Domain.Models;
using CreateAuthor;
using DeleteAuthor;
using GetAllAuthors;
using UpdateAuthor;
using FilterAuthors;
using GetAuthorById;

namespace Application.Authors.Mappers
{
    public static class AuthorMapper
    {
        public static CreateAuthorCommandOutput ToOutput(this Author author)
        {
            return new CreateAuthorCommandOutput
            {
                Id = author.Id,
                Name = author.Name,
                Bio = author.Bio,
                Nationality = string.IsNullOrWhiteSpace(author.Nationality) ? null : author.Nationality,
                BirthDate = author.BirthDate,
                DeathDate = author.DeathDate,
                Genres = author.Genres?.ToArray() ?? Array.Empty<string>()
            };
        }

        public static DeleteAuthorCommandOutput ToDeleteOutput(this Author? author, bool success, string? message = null)
        {
            return new DeleteAuthorCommandOutput
            {
                Success = success,
                Message = message
            };
        }

        public static GetAllAuthorsQueryOutput ToGetAllAuthorsOutput(this Author author)
        {
            return new GetAllAuthorsQueryOutput
            {
                Id = author.Id,
                Name = author.Name,
                Bio = author.Bio,
                Nationality = string.IsNullOrWhiteSpace(author.Nationality) ? null : author.Nationality,
                BirthDate = author.BirthDate,
                DeathDate = author.DeathDate,
                Genres = author.Genres?.ToArray() ?? Array.Empty<string>()
            };
        }

        public static UpdateAuthorCommandOutput ToUpdateAuthorOutput(this Author author)
        {
            return new UpdateAuthorCommandOutput
            {
                Id = author.Id,
                Name = author.Name,
                Bio = author.Bio,
                Nationality = string.IsNullOrWhiteSpace(author.Nationality) ? null : author.Nationality,
                BirthDate = author.BirthDate,
                DeathDate = author.DeathDate,
                Genres = author.Genres?.ToArray() ?? Array.Empty<string>()
            };
        }

        public static FilterAuthorsQueryOutput ToFilterAuthorsOutput(this Author author)
        {
            return new FilterAuthorsQueryOutput
            {
                Id = author.Id,
                Name = author.Name,
                Bio = author.Bio,
                Nationality = string.IsNullOrWhiteSpace(author.Nationality) ? null : author.Nationality,
                BirthDate = author.BirthDate,
                DeathDate = author.DeathDate,
                Genres = author.Genres?.ToArray() ?? Array.Empty<string>()
            };
        }

        public static GetAuthorByIdQueryOutput ToGetAuthorByIdOutput(this Author author)
        {
            return new GetAuthorByIdQueryOutput
            {
                Id = author.Id,
                Name = author.Name,
                Bio = author.Bio,
                Nationality = string.IsNullOrWhiteSpace(author.Nationality) ? null : author.Nationality,
                BirthDate = author.BirthDate,
                DeathDate = author.DeathDate,
                Genres = author.Genres?.ToArray() ?? Array.Empty<string>()
            };
        }
    }

}
