using Domain.Models;
using GetAllReaders;
using GetReaderById;
using FilterReaders;
using CreateReader;
using UpdateReader;
using DeleteReader;

namespace Application.Readers.Mappers
{
    public static class ReaderMapper
    {
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

        public static DeleteReaderCommandOutput ToDeleteReaderOutput(this Reader? reader, bool success, string? message = null)
        {
            return new DeleteReaderCommandOutput
            {
                Success = success,
                Message = message
            };
        }
    }
}
