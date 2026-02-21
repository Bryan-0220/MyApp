using Application.Interfaces;
using Application.Readers.Mappers;

namespace GetAllReaders
{
    public class GetAllReadersQueryHandler : IGetAllReadersQueryHandler
    {
        private readonly IReaderRepository _readerRepository;

        public GetAllReadersQueryHandler(IReaderRepository readerRepository)
        {
            _readerRepository = readerRepository;
        }

        public async Task<IEnumerable<GetAllReadersQueryOutput>> Handle(GetAllReadersQueryInput query, CancellationToken ct = default)
        {
            var readers = await _readerRepository.GetAll(ct);
            return readers.Select(reader => reader.ToGetAllReadersOutput());
        }
    }
}
