using Domain.Models;
using Domain.Results;
using GetAllBooks;
using GetBookById;
using FilterBooks;
using CreateBook;
using UpdateBook;
using DeleteBook;

namespace Application.Books.Mappers
{
    public static class BookMapper
    {
        public static BookData ToData(this CreateBookCommandInput input)
        {
            if (input == null) return null!;

            return new BookData
            {
                Title = input.Title?.Trim() ?? string.Empty,
                AuthorId = input.AuthorId?.Trim() ?? string.Empty,
                ISBN = string.IsNullOrWhiteSpace(input.ISBN) ? null : input.ISBN.Trim(),
                PublishedYear = input.PublishedYear,
                CopiesAvailable = input.CopiesAvailable,
                Genre = input.Genre?.Trim() ?? string.Empty
            };
        }
        public static GetAllBooksQueryOutput ToGetAllBooksOutput(this Book book)
        {
            return new GetAllBooksQueryOutput
            {
                Id = book.Id,
                Title = book.Title,
                AuthorId = book.AuthorId,
                ISBN = book.ISBN,
                PublishedYear = book.PublishedYear,
                CopiesAvailable = book.CopiesAvailable,
                Genre = book.Genre ?? string.Empty
            };
        }

        public static FilterBooksQueryOutput ToFilterBooksOutput(this Book book)
        {
            return new FilterBooksQueryOutput
            {
                Id = book.Id,
                Title = book.Title,
                AuthorId = book.AuthorId,
                ISBN = book.ISBN,
                PublishedYear = book.PublishedYear,
                CopiesAvailable = book.CopiesAvailable,
                Genre = book.Genre ?? string.Empty
            };
        }

        public static CreateBookCommandOutput ToCreateBookOutput(this Book book)
        {
            return new CreateBookCommandOutput
            {
                Id = book.Id,
                Title = book.Title,
                AuthorId = book.AuthorId,
                ISBN = book.ISBN,
                PublishedYear = book.PublishedYear,
                CopiesAvailable = book.CopiesAvailable,
                Genre = book.Genre ?? string.Empty
            };
        }

        public static UpdateBookCommandOutput ToUpdateBookOutput(this Book book)
        {
            return new UpdateBookCommandOutput
            {
                Id = book.Id,
                Title = book.Title,
                AuthorId = book.AuthorId,
                ISBN = book.ISBN,
                PublishedYear = book.PublishedYear,
                CopiesAvailable = book.CopiesAvailable
            };
        }

        public static GetBookByIdQueryOutput ToGetBookByIdOutput(this Book book)
        {
            return new GetBookByIdQueryOutput
            {
                Id = book.Id,
                Title = book.Title,
                AuthorId = book.AuthorId,
                ISBN = book.ISBN,
                PublishedYear = book.PublishedYear,
                CopiesAvailable = book.CopiesAvailable,
                Genre = book.Genre
            };
        }

        public static DeleteBookCommandOutput ToDeleteBookOutput(this Result<Book> result)
        {
            return new DeleteBookCommandOutput
            {
                Success = result.Success,
                Message = result.Message,
                BookId = result.Value?.Id
            };
        }
    }
}
