using Application.Interfaces;
using Domain.Models;
using Domain.Common;
using FluentValidation;

namespace UpdateAuthor
{
    public class UpdateAuthorCommandHandler : IUpdateAuthorCommandHandler
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IValidator<UpdateAuthorCommandInput> _validator;

        public UpdateAuthorCommandHandler(IAuthorRepository authorRepository, IValidator<UpdateAuthorCommandInput> validator)
        {
            _authorRepository = authorRepository;
            _validator = validator;
        }

        public async Task<UpdateAuthorCommandOutput?> HandleAsync(UpdateAuthorCommandInput input, CancellationToken ct = default)
        {
            await _validator.ValidateAndThrowAsync(input, ct);

            var existing = await _authorRepository.GetByIdAsync(input.Id, ct);
            if (existing is null) return null;

            try
            {
                applyAttributes(input, existing);
            }
            catch (DomainException ex)
            {
                throw new InvalidOperationException(ex.Message);
            }

            await _authorRepository.UpdateAsync(existing, ct);

            return new UpdateAuthorCommandOutput
            {
                Id = existing.Id,
                Name = existing.Name,
                Bio = existing.Bio,
                Nationality = string.IsNullOrWhiteSpace(existing.Nationality) ? null : existing.Nationality,
                BirthDate = existing.BirthDate,
                DeathDate = existing.DeathDate,
                Genres = existing.Genres == null ? Array.Empty<string>() : Enumerable.ToArray(existing.Genres)
            };
        }

        private static void applyAttributes(UpdateAuthorCommandInput input, Author existing)
        {
            if (!string.IsNullOrWhiteSpace(input.Name) && input.Name != "string")
                existing.SetName(input.Name!.Trim());

            if (!string.IsNullOrWhiteSpace(input.Bio) && input.Bio != "string")
                existing.SetBio(input.Bio!.Trim());

            if (!string.IsNullOrWhiteSpace(input.Nationality) && input.Nationality != "string")
                existing.SetNationality(input.Nationality!.Trim());

            if (input.BirthDate.HasValue)
                existing.SetBirthDate(input.BirthDate);

            if (input.DeathDate.HasValue)
                existing.SetDeathDate(input.DeathDate);

            if (input.Genres != null)
                existing.SetGenres(input.Genres);
        }
    }
}
