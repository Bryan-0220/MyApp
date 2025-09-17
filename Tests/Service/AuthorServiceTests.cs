using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using System.Collections.Generic;
using Xunit;
using Application.Authors.Services;
using Application.Interfaces;
using Domain.Models;

namespace Tests.Service
{
    public class AuthorServiceTests
    {
        [Fact]
        public async Task GivenAuthorDoesNotExist_WhenDeleteAuthor_ThenReturnsFail()
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
        public async Task GivenAuthorExistsAndNoBooks_WhenDeleteAuthor_ThenDeletesAndReturnsOk()
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
    }
}
