using Application.Interfaces;
using Domain.Models;
using FluentValidation;

namespace UpdateBook
{
    public class UpdateBookCommandHandler : IUpdateBookCommandHandler
    {
        private readonly IBookRepository _bookRepository;
        private readonly IValidator<UpdateBookCommandInput> _validator;

        public UpdateBookCommandHandler(IBookRepository bookRepository, IValidator<UpdateBookCommandInput> validator)
        {
            _bookRepository = bookRepository;
            _validator = validator;
        }

        public async Task<UpdateBookCommandOutput?> HandleAsync(UpdateBookCommandInput input, CancellationToken ct = default)
        {
            await _validator.ValidateAndThrowAsync(input, ct);

            var existing = await _bookRepository.GetByIdAsync(input.Id, ct);
            if (existing is null) return null;

            try
            {
                applyAttributes(input, existing);
            }
            catch (Domain.Common.DomainException ex)
            {
                throw new InvalidOperationException(ex.Message);
            }

            await _bookRepository.UpdateAsync(existing, ct);

            return new UpdateBookCommandOutput
            {
                Id = existing.Id,
                Title = existing.Title,
                AuthorId = existing.AuthorId,
                ISBN = existing.ISBN,
                PublishedYear = existing.PublishedYear,
                CopiesAvailable = existing.CopiesAvailable
            };
        }

        private static void applyAttributes(UpdateBookCommandInput input, Book existing)
        {
            if (!string.IsNullOrWhiteSpace(input.Title) && input.Title != "string")
                existing.SetTitle(input.Title!.Trim());

            if (!string.IsNullOrWhiteSpace(input.AuthorId) && input.AuthorId != "string")
                existing.SetAuthor(input.AuthorId!.Trim());

            if (!string.IsNullOrWhiteSpace(input.ISBN) && input.ISBN != "string")
                existing.SetIsbn(input.ISBN!.Trim());

            if (input.PublishedYear.HasValue && input.PublishedYear.Value != -1)
                existing.SetPublishedYear(input.PublishedYear);

            if (input.CopiesAvailable.HasValue && input.CopiesAvailable.Value != -1)
                existing.SetCopiesAvailable(input.CopiesAvailable.Value);
        }
    }
}
