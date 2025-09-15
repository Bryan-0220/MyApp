using Domain.Models;
using GetAllBooks;
using FilterBooks;
using CreateBook;
using UpdateBook;
using DeleteBook;

namespace Application.Books.Mappers
{
    public static class BookMapper
    {
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

        public static DeleteBookCommandOutput ToDeleteBookOutput(this Book? book, bool success, string? message = null)
        {
            return new DeleteBookCommandOutput
            {
                Success = success,
                Message = message
            };
        }
    }
}
