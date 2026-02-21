using Application.Interfaces;
using Domain.Results;
using Application.Readers.Mappers;

namespace GetReaderById
{
    public class GetReaderByIdQueryHandler : IGetReaderByIdQueryHandler
    {
        private readonly IReaderRepository _readerRepository;

        public GetReaderByIdQueryHandler(IReaderRepository readerRepository)
        {
            _readerRepository = readerRepository;
        }

        public async Task<Result<GetReaderByIdQueryOutput>> Handle(GetReaderByIdQueryInput query, CancellationToken ct = default)
        {
            var reader = await _readerRepository.GetById(query.Id, ct);
            if (reader is null) return Result<GetReaderByIdQueryOutput>.Fail("Reader not found");
            return Result<GetReaderByIdQueryOutput>.Ok(reader.ToGetReaderByIdOutput());
        }
    }
}
