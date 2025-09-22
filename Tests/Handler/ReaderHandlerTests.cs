using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using FluentValidation;
using Xunit;

using FilterReaders;
using GetReaderById;
using GetAllReaders;
using CreateReader;
using DeleteReader;

using Application.Interfaces;
using Application.Readers.Services;
using Domain.Models;
using Domain.Results;

namespace Tests.Handler
{
    public class ReaderHandlerTests
    {
        [Fact]
        public async Task FilterReaders_Handler_throws_when_firstname_is_empty_string()
        {
            // Arrange
            var repo = A.Fake<IReaderRepository>();
            var validator = new FilterReaders.FilterReadersQueryValidator();
            var handler = new FilterReaders.FilterReadersQueryHandler(repo, validator);

            var input = new FilterReadersQueryInput { FirstName = "" }; // empty but not null -> should fail validator

            // Act
            Func<Task> act = async () => await handler.Handle(input, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ValidationException>().WithMessage("*FirstName must not be empty if provided.*");
        }

        [Fact]
        public async Task FilterReaders_Handler_maps_repository_results_to_output()
        {
            // Arrange
            var repo = A.Fake<IReaderRepository>();
            var validator = new FilterReaders.FilterReadersQueryValidator();
            var handler = new FilterReaders.FilterReadersQueryHandler(repo, validator);

            var reader = new Reader
            {
                Id = "r-1",
                FirstName = "Ana",
                LastName = "Gomez",
                Email = "ana@example.com",
                MembershipDate = DateOnly.FromDateTime(new DateTime(2020, 5, 1))
            };

            A.CallTo(() => repo.Filter(A<Application.Filters.ReaderFilter?>._, A<CancellationToken>._))
                .Returns(new[] { reader });

            var input = new FilterReadersQueryInput { FirstName = "Ana" };

            // Act
            var result = await handler.Handle(input, CancellationToken.None);

            // Assert
            result.Should().HaveCount(1);
            var outItem = result.First();
            outItem.Id.Should().Be(reader.Id);
            outItem.FirstName.Should().Be(reader.FirstName);
            outItem.LastName.Should().Be(reader.LastName);
            outItem.Email.Should().Be(reader.Email);
            outItem.MembershipDate.Should().Be(reader.MembershipDate);
        }

        [Fact]
        public async Task CreateReader_Handler_trims_input_before_calling_service()
        {
            // Arrange
            var service = A.Fake<IReaderService>();
            var validator = A.Fake<IValidator<CreateReaderCommandInput>>();
            A.CallTo(() => validator.ValidateAsync(A<CreateReaderCommandInput>._, A<CancellationToken>._))
                .Returns(Task.FromResult(new FluentValidation.Results.ValidationResult()));

            var handler = new CreateReader.CreateReaderCommandHandler(service, validator);

            var input = new CreateReaderCommandInput
            {
                FirstName = "  José  ",
                LastName = "  Pérez  ",
                Email = "  JOSE.PEREZ@EXAMPLE.COM  ",
                MembershipDate = DateOnly.FromDateTime(new DateTime(2021, 1, 2))
            };

            // The service should be called with trimmed values (Mapper trims). Capture the passed ReaderData
            A.CallTo(() => service.CreateReader(A<ReaderData>._, A<CancellationToken>._))
                .ReturnsLazily((ReaderData rd, CancellationToken ct) => Task.FromResult(new Reader
                {
                    Id = "new-id",
                    FirstName = rd.FirstName,
                    LastName = rd.LastName,
                    Email = rd.Email,
                    MembershipDate = rd.MembershipDate!.Value
                }));

            // Act
            var output = await handler.Handle(input, CancellationToken.None);

            // Assert - ensure the values saved are trimmed (no leading/trailing spaces) and email preserved in case
            A.CallTo(() => service.CreateReader(A<ReaderData>.That.Matches(rd =>
                rd.FirstName == "José" && rd.LastName == "Pérez" && rd.Email == "JOSE.PEREZ@EXAMPLE.COM"), A<CancellationToken>._))
                .MustHaveHappenedOnceExactly();

            output.Id.Should().Be("new-id");
            output.FirstName.Should().Be("José");
            output.Email.Should().Be("JOSE.PEREZ@EXAMPLE.COM");
        }

        [Fact]
        public async Task DeleteReader_Handler_maps_failed_service_result_to_output()
        {
            // Arrange
            var service = A.Fake<IReaderService>();
            var validator = A.Fake<IValidator<DeleteReaderCommandInput>>();
            A.CallTo(() => validator.ValidateAsync(A<DeleteReaderCommandInput>._, A<CancellationToken>._))
                .Returns(Task.FromResult(new FluentValidation.Results.ValidationResult()));

            var handler = new DeleteReader.DeleteReaderCommandHandler(service, validator);

            var input = new DeleteReaderCommandInput { Id = "r-42" };

            var failResult = Result<Reader>.Fail("Reader has active loans");
            A.CallTo(() => service.DeleteReader(input.Id, A<CancellationToken>._))
                .Returns(Task.FromResult(failResult));

            // Act
            var output = await handler.Handle(input, CancellationToken.None);

            // Assert
            output.Success.Should().BeFalse();
            output.Message.Should().Be("Reader has active loans");
            output.ReaderId.Should().BeNull();
        }

        [Fact]
        public async Task GetAllReaders_Handler_maps_all_readers()
        {
            // Arrange
            var repo = A.Fake<IReaderRepository>();
            var handler = new GetAllReaders.GetAllReadersQueryHandler(repo);

            var r1 = new Reader { Id = "a", FirstName = "A", LastName = "A", Email = "a@ex.com", MembershipDate = DateOnly.FromDateTime(new DateTime(2019, 1, 1)) };
            var r2 = new Reader { Id = "b", FirstName = "B", LastName = "B", Email = "b@ex.com", MembershipDate = DateOnly.FromDateTime(new DateTime(2020, 2, 2)) };

            A.CallTo(() => repo.GetAll(A<CancellationToken>._)).Returns(new[] { r1, r2 });

            // Act
            var output = await handler.Handle(new GetAllReaders.GetAllReadersQueryInput(), CancellationToken.None);

            // Assert
            output.Should().HaveCount(2);
            output.Select(x => x.Id).Should().BeEquivalentTo(new[] { "a", "b" });
        }

        [Fact]
        public async Task GetReaderById_Handler_returns_fail_when_not_found()
        {
            // Arrange
            var repo = A.Fake<IReaderRepository>();
            var handler = new GetReaderById.GetReaderByIdQueryHandler(repo);

            A.CallTo(() => repo.GetById(A<string>._, A<CancellationToken>._)).Returns(Task.FromResult<Reader?>(null));

            // Act
            var res = await handler.Handle(new GetReaderByIdQueryInput { Id = Guid.NewGuid().ToString() }, CancellationToken.None);

            // Assert
            res.Success.Should().BeFalse();
            res.Message.Should().Be("Reader not found");
            res.Value.Should().BeNull();
        }

        [Fact]
        public async Task GetReaderById_Handler_maps_reader_when_found()
        {
            // Arrange
            var repo = A.Fake<IReaderRepository>();
            var handler = new GetReaderById.GetReaderByIdQueryHandler(repo);

            var reader = new Reader { Id = Guid.NewGuid().ToString(), FirstName = "X", LastName = "Y", Email = "xy@ex.com", MembershipDate = DateOnly.FromDateTime(new DateTime(2018, 3, 3)) };
            A.CallTo(() => repo.GetById(reader.Id, A<CancellationToken>._)).Returns(Task.FromResult<Reader?>(reader));

            // Act
            var res = await handler.Handle(new GetReaderByIdQueryInput { Id = reader.Id }, CancellationToken.None);

            // Assert
            res.Success.Should().BeTrue();
            res.Value.Should().NotBeNull();
            res.Value!.Id.Should().Be(reader.Id);
            res.Value.FirstName.Should().Be(reader.FirstName);
        }

        [Fact]
        public async Task UpdateReader_Handler_throws_when_membershipdate_in_future()
        {
            // Arrange
            var service = A.Fake<IReaderService>();
            var validator = new UpdateReader.UpdateReaderCommandValidator();
            var handler = new UpdateReader.UpdateReaderCommandHandler(service, validator);

            var input = new UpdateReader.UpdateReaderCommandInput
            {
                Id = Guid.NewGuid().ToString(),
                MembershipDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)) // future
            };

            // Act
            Func<Task> act = async () => await handler.Handle(input, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<FluentValidation.ValidationException>().WithMessage("*MembershipDate cannot be in the future*");
        }

        [Fact]
        public async Task UpdateReader_Handler_calls_service_and_maps_result()
        {
            // Arrange
            var service = A.Fake<IReaderService>();
            var validator = new UpdateReader.UpdateReaderCommandValidator();
            var handler = new UpdateReader.UpdateReaderCommandHandler(service, validator);

            var input = new UpdateReader.UpdateReaderCommandInput
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = "New",
                LastName = "Name",
                Email = "new@ex.com",
                MembershipDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1))
            };

            var updated = new Reader { Id = input.Id, FirstName = input.FirstName!, LastName = input.LastName!, Email = input.Email!, MembershipDate = input.MembershipDate!.Value };
            A.CallTo(() => service.UpdateReader(A<UpdateReader.UpdateReaderCommandInput>._, A<CancellationToken>._)).Returns(Task.FromResult(updated));

            // Act
            var output = await handler.Handle(input, CancellationToken.None);

            // Assert
            output.Id.Should().Be(input.Id);
            output.FirstName.Should().Be(input.FirstName);
            output.Email.Should().Be(input.Email);
        }
    }
}
