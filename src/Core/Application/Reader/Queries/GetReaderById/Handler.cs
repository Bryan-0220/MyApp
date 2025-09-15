using Application.Interfaces;
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

        public async Task<GetReaderByIdQueryOutput?> HandleAsync(GetReaderByIdQueryInput query, CancellationToken ct = default)
        {
            var user = await _readerRepository.GetByIdAsync(query.Id, ct);
            if (user is null) return null;

            return user.ToGetReaderByIdOutput();
        }
    }
}
