using Application.Interfaces;
using Application.Filters;
using FluentValidation;

namespace FilterBooks
{
    public class FilterBooksQueryHandler : IFilterBooksQueryHandler
    {
        private readonly IBookRepository _bookRepository;
        private readonly IValidator<FilterBooksQueryInput> _validator;

        public FilterBooksQueryHandler(IBookRepository bookRepository, IValidator<FilterBooksQueryInput> validator)
        {
            _bookRepository = bookRepository;
            _validator = validator;
        }

        public async Task<IEnumerable<FilterBooksQueryOutput>> HandleAsync(FilterBooksQueryInput input, CancellationToken ct = default)
        {
            await _validator.ValidateAndThrowAsync(input, ct);

            var filter = input.ToBookFilter();
            var books = await _bookRepository.FilterAsync(filter.ToExpression(), ct);

            return books.Select(book => new FilterBooksQueryOutput
            {
                Id = book.Id,
                Title = book.Title,
                AuthorId = book.AuthorId,
                ISBN = book.ISBN,
                PublishedYear = book.PublishedYear,
                CopiesAvailable = book.CopiesAvailable,
                Genre = book.Genre
            });
        }
    }
}
