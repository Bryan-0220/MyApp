using Domain.Models;
using GetAllReaders;
using GetReaderById;
using FilterReaders;
using CreateReader;
using UpdateReader;
using DeleteReader;
using Domain.Results;

namespace Application.Readers.Mappers
{
    public static class ReaderMapper
    {
        public static ReaderData ToData(this CreateReaderCommandInput input)
        {
            if (input == null) return null!;

            return new ReaderData
            {
                FirstName = input.FirstName?.Trim() ?? string.Empty,
                LastName = input.LastName?.Trim() ?? string.Empty,
                Email = input.Email?.Trim() ?? string.Empty,
                MembershipDate = input.MembershipDate
            };
        }

        public static GetAllReadersQueryOutput ToGetAllReadersOutput(this Reader reader)
        {
            return new GetAllReadersQueryOutput
            {
                Id = reader.Id,
                FirstName = reader.FirstName,
                LastName = reader.LastName,
                Email = reader.Email,
                MembershipDate = reader.MembershipDate
            };
        }

        public static GetReaderByIdQueryOutput ToGetReaderByIdOutput(this Reader reader)
        {
            return new GetReaderByIdQueryOutput
            {
                Id = reader.Id,
                FirstName = reader.FirstName,
                LastName = reader.LastName,
                Email = reader.Email,
                MembershipDate = reader.MembershipDate
            };
        }

        public static FilterReadersQueryOutput ToFilterReadersOutput(this Reader reader)
        {
            return new FilterReadersQueryOutput
            {
                Id = reader.Id,
                FirstName = reader.FirstName,
                LastName = reader.LastName,
                Email = reader.Email,
                MembershipDate = reader.MembershipDate
            };
        }

        public static CreateReaderCommandOutput ToCreateReaderOutput(this Reader reader)
        {
            return new CreateReaderCommandOutput
            {
                Id = reader.Id,
                FirstName = reader.FirstName,
                LastName = reader.LastName,
                Email = reader.Email,
                MembershipDate = reader.MembershipDate
            };
        }

        public static UpdateReaderCommandOutput ToUpdateReaderOutput(this Reader reader)
        {
            return new UpdateReaderCommandOutput
            {
                Id = reader.Id,
                FirstName = reader.FirstName,
                LastName = reader.LastName,
                Email = reader.Email,
                MembershipDate = reader.MembershipDate
            };
        }

        public static DeleteReaderCommandOutput ToDeleteReaderOutput(this Result<Reader> result)
        {
            return new DeleteReaderCommandOutput
            {
                Success = result.Success,
                Message = result.Message,
                ReaderId = result.Value?.Id
            };
        }
    }
}
