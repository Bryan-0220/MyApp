using Domain.Models;
using Domain.Results;
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
        public static AuthorData ToData(this CreateAuthorCommandInput input)
        {
            if (input == null) return null!;

            return new AuthorData
            {
                Name = input.Name?.Trim() ?? string.Empty,
                Bio = string.IsNullOrWhiteSpace(input.Bio) ? null : input.Bio.Trim(),
                Nationality = string.IsNullOrWhiteSpace(input.Nationality) ? null : input.Nationality.Trim(),
                BirthDate = input.BirthDate,
                DeathDate = input.DeathDate,
                Genres = input.Genres?.Where(g => !string.IsNullOrWhiteSpace(g)).Select(g => g.Trim()).ToArray() ?? Array.Empty<string>()
            };
        }

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


        public static DeleteAuthorCommandOutput ToDeleteOutput(this Result<Author> result)
        {
            return new DeleteAuthorCommandOutput
            {
                Success = result.Success,
                Message = result.Message,
                AuthorId = result.Value?.Id
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
