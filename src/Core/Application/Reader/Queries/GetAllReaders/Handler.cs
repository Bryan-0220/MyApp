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

        public async Task<IEnumerable<GetAllReadersQueryOutput>> HandleAsync(GetAllReadersQueryInput query, CancellationToken ct = default)
        {
            var users = await _readerRepository.GetAllAsync(ct);

            var projected = users.Select(r => r.ToGetAllReadersOutput());

            return projected;
        }
    }
}
