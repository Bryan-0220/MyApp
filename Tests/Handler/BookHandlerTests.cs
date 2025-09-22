using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentValidation;
using FluentValidation.Results;
using CreateBook;
using UpdateBook;
using DeleteBook;
using GetBookById;
using GetAllBooks;
using FilterBooks;
using Application.Interfaces;
using Application.Books.Services;
using Application.Filters;
using Domain.Models;
using Domain.Common;
using FakeItEasy;

namespace Tests.Handler
{
    public class BookHandlerTests
    {
        [Fact]
        public async Task CreateBookHandler_ShouldTrimFields_AndHandleNullIsbn()
        {
            var repo = A.Fake<IBookRepository>();
            var validator = A.Fake<FluentValidation.IValidator<CreateBookCommandInput>>();

            Book? captured = null;
            A.CallTo(() => repo.Create(A<Book>._, A<CancellationToken>._))
                .Invokes((Book b, CancellationToken ct) => captured = b)
                .ReturnsLazily((Book b, CancellationToken ct) => Task.FromResult(b));

            var handler = new CreateBookCommandHandler(repo, validator);

            var input = new CreateBookCommandInput
            {
                Title = "  My Title  ",
                AuthorId = "  author1  ",
                ISBN = "   ",
                CopiesAvailable = 3,
                Genre = " Sci-Fi "
            };

            var output = await handler.Handle(input);

            Assert.NotNull(captured);
            Assert.Equal("My Title", captured!.Title);
            Assert.Equal("author1", captured.AuthorId);
            Assert.Null(captured.ISBN);
            Assert.Equal(3, captured.CopiesAvailable);
            Assert.Equal("Sci-Fi", captured.Genre);

            Assert.Equal(captured.Id, output.Id);
            Assert.Equal(captured.Title, output.Title);
        }

        [Fact]
        public async Task CreateBookHandler_ShouldBubbleRepositoryException()
        {
            var repo = A.Fake<IBookRepository>();
            var validator = A.Fake<FluentValidation.IValidator<CreateBookCommandInput>>();

            A.CallTo(() => repo.Create(A<Book>._, A<CancellationToken>._))
                .Throws(new InvalidOperationException("DB down"));

            var handler = new CreateBookCommandHandler(repo, validator);

            var input = new CreateBookCommandInput { Title = "T", AuthorId = "a", Genre = "g" };

            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(input));
        }

        [Fact]
        public async Task UpdateBookHandler_ShouldApplyPartialUpdate_AndReturnMappedOutput()
        {
            var service = A.Fake<IBookService>();
            var validator = A.Fake<FluentValidation.IValidator<UpdateBookCommandInput>>();

            var existing = Book.Create(new BookData { Title = "Old", AuthorId = "a", CopiesAvailable = 2, Genre = "g", ISBN = "12345" });
            existing.Id = "b1";

            // Simulate service returning updated book where only Title changed
            A.CallTo(() => service.UpdateBook(A<UpdateBookCommandInput>._, A<CancellationToken>._))
                .ReturnsLazily((UpdateBookCommandInput inp, CancellationToken ct) =>
                {
                    if (!string.IsNullOrWhiteSpace(inp.Title)) existing.SetTitle(inp.Title);
                    if (inp.CopiesAvailable.HasValue) existing.SetCopiesAvailable(inp.CopiesAvailable.Value);
                    return Task.FromResult(existing);
                });

            var handler = new UpdateBook.UpdateBookCommandHandler(service, validator);

            var input = new UpdateBookCommandInput { Id = "b1", Title = " New Title " };
            var output = await handler.Handle(input);

            // Ensure service was called and mapping applied
            A.CallTo(() => service.UpdateBook(A<UpdateBookCommandInput>._, A<CancellationToken>._)).MustHaveHappened();
            Assert.Equal("New Title", output.Title);
            Assert.Equal(existing.Id, output.Id);
            // CopiesAvailable should remain unchanged
            Assert.Equal(2, output.CopiesAvailable);
        }

        [Fact]
        public async Task UpdateBookHandler_ShouldPropagateDuplicateException_FromService()
        {
            var service = A.Fake<IBookService>();
            var validator = A.Fake<FluentValidation.IValidator<UpdateBookCommandInput>>();

            A.CallTo(() => service.UpdateBook(A<UpdateBookCommandInput>._, A<CancellationToken>._))
                .Throws(new DuplicateException("ISBN already used"));

            var handler = new UpdateBook.UpdateBookCommandHandler(service, validator);

            var input = new UpdateBookCommandInput { Id = "b1", ISBN = "dup-isbn" };

            await Assert.ThrowsAsync<DuplicateException>(() => handler.Handle(input));
        }

        [Fact]
        public async Task DeleteBookHandler_ShouldReturnMappedFailure_WhenServiceReturnsFailure()
        {
            var service = A.Fake<IBookService>();
            var validator = A.Fake<FluentValidation.IValidator<DeleteBookCommandInput>>();

            var failure = global::Domain.Results.Result<Book>.Fail("Cannot delete");
            A.CallTo(() => service.DeleteBook(A<string>._, A<CancellationToken>._)).Returns(Task.FromResult(failure));

            var handler = new DeleteBookCommandHandler(validator, service);

            var input = new DeleteBookCommandInput { Id = "b1" };
            var output = await handler.Handle(input);

            Assert.False(output.Success);
            Assert.Equal("Cannot delete", output.Message);
            Assert.Null(output.BookId);
        }

        [Fact]
        public async Task GetBookById_ShouldReturnFail_WhenMissing()
        {
            var repo = A.Fake<IBookRepository>();
            A.CallTo(() => repo.GetById(A<string>._, A<CancellationToken>._)).Returns(Task.FromResult<Book?>(null));

            var handler = new GetBookById.GetBookByIdQueryHandler(repo);
            var input = new GetBookByIdQueryInput { Id = "missing" };

            var result = await handler.Handle(input);

            Assert.False(result.Success);
            Assert.Equal("Book not found", result.Message);
        }

        [Fact]
        public async Task GetBookById_ShouldMapGenreProperly()
        {
            var repo = A.Fake<IBookRepository>();
            var book = Book.Create(new BookData { Title = "T", AuthorId = "a", CopiesAvailable = 1, Genre = "g", ISBN = "12345" });
            book.Id = "b1";

            A.CallTo(() => repo.GetById("b1", A<CancellationToken>._)).Returns(Task.FromResult<Book?>(book));

            var handler = new GetBookById.GetBookByIdQueryHandler(repo);
            var input = new GetBookByIdQueryInput { Id = "b1" };

            var result = await handler.Handle(input);

            Assert.True(result.Success);
            Assert.NotNull(result.Value);
            Assert.Equal("g", result.Value!.Genre);
        }

        [Fact]
        public async Task GetAllBooks_ShouldReturnEmptyEnumerable_WhenNoBooks()
        {
            var repo = A.Fake<IBookRepository>();
            A.CallTo(() => repo.GetAll(A<CancellationToken>._)).Returns(Task.FromResult<IEnumerable<Book>>(new List<Book>()));

            var handler = new GetAllBooks.GetAllBooksQueryHandler(repo);
            var input = new GetAllBooksQueryInput();

            var result = await handler.Handle(input);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task FilterBooks_ShouldCallValidator_AndMapResults()
        {
            var repo = A.Fake<IBookRepository>();
            var validator = A.Fake<IValidator<FilterBooksQueryInput>>();

            var books = new List<Book> {
                Book.Create(new BookData { Title = "T1", AuthorId = "a", CopiesAvailable = 1, Genre = "g1", ISBN = "12345" }),
                Book.Create(new BookData { Title = "T2", AuthorId = "b", CopiesAvailable = 2, Genre = "g2", ISBN = "54321" })
            };

            A.CallTo(() => repo.Filter(A<BookFilter>._, A<CancellationToken>._)).Returns(books);
            // Arrange validator to return successful ValidationResult when ValidateAsync is called
            A.CallTo(() => validator.ValidateAsync(A<FluentValidation.ValidationContext<FilterBooksQueryInput>>._, A<CancellationToken>._))
                .Returns(Task.FromResult(new FluentValidation.Results.ValidationResult()));

            var handler = new FilterBooks.FilterBooksQueryHandler(repo, validator);

            var input = new FilterBooksQueryInput { Genre = "g1" };
            var result = await handler.Handle(input);

            // ValidateAsync is the method called by the extension ValidateAndThrowAsync (it passes a ValidationContext<T>)
            A.CallTo(() => validator.ValidateAsync(A<FluentValidation.ValidationContext<FilterBooksQueryInput>>._, A<CancellationToken>._)).MustHaveHappened();
            Assert.Equal(2, result.Count());
            Assert.Contains(result, r => r.Title == "T1");
        }

        [Fact]
        public async Task FilterBooks_ShouldBubbleValidationException()
        {
            var repo = A.Fake<IBookRepository>();
            var validator = A.Fake<IValidator<FilterBooksQueryInput>>();

            // Make ValidateAsync return an invalid ValidationResult so ValidateAndThrowAsync will throw
            var failures = new[] { new FluentValidation.Results.ValidationFailure("Genre", "invalid") };
            var invalid = new FluentValidation.Results.ValidationResult(failures);
            A.CallTo(() => validator.ValidateAsync(A<FluentValidation.ValidationContext<FilterBooksQueryInput>>._, A<CancellationToken>._))
                .Throws(new FluentValidation.ValidationException(invalid.Errors));

            var handler = new FilterBooks.FilterBooksQueryHandler(repo, validator);
            var input = new FilterBooksQueryInput { Genre = "g" };

            await Assert.ThrowsAsync<FluentValidation.ValidationException>(() => handler.Handle(input));
        }
    }
}
