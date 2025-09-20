using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FakeItEasy;

using CreateAuthor;
using UpdateAuthor;
using DeleteAuthor;
using GetAllAuthors;
using GetAuthorById;
using FilterAuthors;
using Application.Authors.Services;
using Application.Interfaces;
using Domain.Models;
using Domain.Results;
using Domain.Common;

namespace Tests.Service
{

    public class AuthorCommandQueryTests
    {
        [Fact]
        public async Task DeleteAuthor_ShouldReturnFail_WhenAuthorDoesNotExist()
        {
            var repo = A.Fake<IAuthorRepository>();
            var bookRepo = A.Fake<IBookRepository>();

            A.CallTo(() => repo.GetById(A<string>._, A<CancellationToken>._)).Returns(Task.FromResult<Author?>(null));

            var service = new AuthorService(repo, bookRepo);

            var result = await service.DeleteAuthor("non-existent", CancellationToken.None);

            Assert.False(result.Success);
            Assert.Equal("Author not found", result.Message);
        }

        [Fact]
        public async Task DeleteAuthor_ShouldDeleteAndReturnOk_WhenAuthorExistsAndNoBooks()
        {
            var repo = A.Fake<IAuthorRepository>();
            var bookRepo = A.Fake<IBookRepository>();

            var author = new Author { Id = "author-1" };

            A.CallTo(() => repo.GetById("author-1", A<CancellationToken>._)).Returns(Task.FromResult<Author?>(author));
            A.CallTo(() => bookRepo.Filter(A<Application.Filters.BookFilter>._, A<CancellationToken>._)).Returns(Task.FromResult<IEnumerable<Book>>(new List<Book>()));
            A.CallTo(() => repo.Delete("author-1", A<CancellationToken>._)).Returns(Task.FromResult(true));

            var service = new AuthorService(repo, bookRepo);

            var result = await service.DeleteAuthor("author-1", CancellationToken.None);

            Assert.True(result.Success);
            Assert.Equal("Author deleted", result.Message);

            A.CallTo(() => repo.Delete("author-1", A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData("", "Test", new[] { "Fiction" })] // Nombre vacío
        [InlineData("Test", "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", new[] { "Fiction" })] // Nacionalidad > 100
        [InlineData("Test", "Test", new[] { "" })] // Género vacío
        public async Task CreateAuthor_ShouldFailValidation(string name, string nationality, string[] genres)
        {
            var repo = A.Fake<IAuthorRepository>();
            var bookRepo = A.Fake<IBookRepository>();
            var service = new AuthorService(repo, bookRepo);
            var validator = new CreateAuthorCommandValidator(repo);
            var handler = new CreateAuthorCommandHandler(validator, service);
            var input = new CreateAuthorCommandInput { Name = name, Nationality = nationality, Genres = genres };
            await Assert.ThrowsAsync<FluentValidation.ValidationException>(() => handler.Handle(input, CancellationToken.None));
        }

        [Fact]
        public async Task CreateAuthor_ShouldFail_WhenBirthDateAfterDeathDate()
        {
            var repo = A.Fake<IAuthorRepository>();
            var bookRepo = A.Fake<IBookRepository>();
            var service = new AuthorService(repo, bookRepo);
            var validator = new CreateAuthorCommandValidator(repo);
            var handler = new CreateAuthorCommandHandler(validator, service);
            var input = new CreateAuthorCommandInput
            {
                Name = "Test",
                Nationality = "Test",
                Genres = new[] { "Fiction" },
                BirthDate = new DateOnly(2000, 1, 1),
                DeathDate = new DateOnly(1990, 1, 1)
            };
            await Assert.ThrowsAsync<FluentValidation.ValidationException>(() => handler.Handle(input, CancellationToken.None));
        }

        [Fact]
        public async Task CreateAuthor_ShouldFail_WhenAuthorWithSameNameExists()
        {
            var repo = A.Fake<IAuthorRepository>();
            var bookRepo = A.Fake<IBookRepository>();
            var service = new AuthorService(repo, bookRepo);
            var handler = new CreateAuthorCommandHandler(A.Fake<FluentValidation.IValidator<CreateAuthorCommandInput>>(), service);
            var input = new CreateAuthorCommandInput { Name = "Test Author", Nationality = "Test", Genres = new[] { "Fiction" } };
            // Simula que ya existe un autor con ese nombre
            A.CallTo(() => repo.Filter(A<Application.Filters.AuthorFilter>._, A<CancellationToken>._)).Returns(new List<Author> { Author.Create(new AuthorData { Name = "Test Author", Nationality = "Test", Genres = new[] { "Fiction" } }) });
            await Assert.ThrowsAsync<DuplicateException>(() => handler.Handle(input, CancellationToken.None));
        }

        [Fact]
        public async Task CreateAuthor_ShouldReturnOutput_WhenInputIsValid()
        {
            var repo = A.Fake<IAuthorRepository>();
            var bookRepo = A.Fake<IBookRepository>();
            var service = new AuthorService(repo, bookRepo);
            var handler = new CreateAuthorCommandHandler(
                A.Fake<FluentValidation.IValidator<CreateAuthorCommandInput>>(),
                service
            );
            var input = new CreateAuthorCommandInput
            {
                Name = "Test Author",
                Nationality = "Testland",
                Genres = new[] { "Fiction" }
            };
            A.CallTo(() => repo.Filter(A<Application.Filters.AuthorFilter>._, A<CancellationToken>._)).Returns(new List<Author>());
            var author = Author.Create(new AuthorData
            {
                Name = "Test Author",
                Nationality = "Testland",
                Genres = new[] { "Fiction" }
            });
            author.Id = "id";
            A.CallTo(() => repo.Create(A<Author>._, A<CancellationToken>._)).Returns(author);
            var output = await handler.Handle(input, CancellationToken.None);
            Assert.Equal("Test Author", output.Name);
            Assert.Equal("Testland", output.Nationality);
            Assert.Contains("Fiction", output.Genres);
        }

        [Fact]
        public async Task UpdateAuthor_ShouldReturnOutput_WhenAuthorExists()
        {
            var repo = A.Fake<IAuthorRepository>();
            var bookRepo = A.Fake<IBookRepository>();
            var service = new AuthorService(repo, bookRepo);
            var handler = new UpdateAuthorCommandHandler(
                service,
                A.Fake<FluentValidation.IValidator<UpdateAuthorCommandInput>>()
            );
            var author = Author.Create(new AuthorData
            {
                Name = "Old Name",
                Nationality = "Old",
                Genres = new[] { "Fiction" }
            });
            author.Id = "id";
            A.CallTo(() => repo.GetById("id", A<CancellationToken>._)).Returns(author);
            A.CallTo(() => repo.Update(A<Author>._, A<CancellationToken>._)).Returns(Task.FromResult(true));
            var input = new UpdateAuthorCommandInput { Id = "id", Name = "New Name", Nationality = "New" };
            var output = await handler.Handle(input, CancellationToken.None);
            Assert.Equal("id", output.Id);
            Assert.Equal("New Name", output.Name);
            Assert.Equal("New", output.Nationality);
        }

        [Fact]
        public async Task GetAllAuthors_ShouldReturnList_WhenAuthorsExist()
        {
            var repo = A.Fake<IAuthorRepository>();
            var handler = new GetAllAuthorsQueryHandler(repo);
            var author = Author.Create(new AuthorData
            {
                Name = "A",
                Nationality = "Test",
                Genres = new[] { "Fiction" }
            });
            author.Id = "id";
            var authors = new List<Author> { author };
            A.CallTo(() => repo.GetAll(A<CancellationToken>._)).Returns(authors);
            var result = await handler.Handle(new GetAllAuthorsQueryInput(), CancellationToken.None);
            Assert.Single(result);
            Assert.Equal("A", result.First().Name);
        }

        [Fact]
        public async Task GetAuthorById_ShouldReturnAuthor_WhenExists()
        {
            var repo = A.Fake<IAuthorRepository>();
            var handler = new GetAuthorByIdQueryHandler(repo);
            var author = Author.Create(new AuthorData
            {
                Name = "A",
                Nationality = "Test",
                Genres = new[] { "Fiction" }
            });
            author.Id = "id";
            A.CallTo(() => repo.GetById("id", A<CancellationToken>._)).Returns(author);
            var result = await handler.Handle(new GetAuthorByIdQueryInput { Id = "id" }, CancellationToken.None);
            Assert.True(result.Success);
            Assert.Equal("A", result.Value?.Name);
        }

        [Fact]
        public async Task GetAuthorById_ShouldReturnFail_WhenNotExists()
        {
            var repo = A.Fake<IAuthorRepository>();
            var handler = new GetAuthorByIdQueryHandler(repo);
            A.CallTo(() => repo.GetById("id", A<CancellationToken>._)).Returns((Author?)null);
            var result = await handler.Handle(new GetAuthorByIdQueryInput { Id = "id" }, CancellationToken.None);
            Assert.False(result.Success);
            Assert.Equal("Author not found", result.Message);
        }

        [Fact]
        public async Task FilterAuthors_ShouldReturnFiltered_WhenCriteriaMatch()
        {
            var repo = A.Fake<IAuthorRepository>();
            var handler = new FilterAuthorsQueryHandler(repo, A.Fake<FluentValidation.IValidator<FilterAuthorsQueryInput>>());
            var author = Author.Create(new AuthorData
            {
                Name = "A",
                Nationality = "Test",
                Genres = new[] { "Fiction" }
            });
            author.Id = "id";
            var authors = new List<Author> { author };
            A.CallTo(() => repo.Filter(A<Application.Filters.AuthorFilter>._, A<CancellationToken>._)).Returns(authors);
            var input = new FilterAuthorsQueryInput { Name = "A", Genres = new[] { "Fiction" } };
            var result = await handler.Handle(input, CancellationToken.None);
            Assert.Single(result);
            Assert.Equal("A", result.First().Name);
        }
    }
}
