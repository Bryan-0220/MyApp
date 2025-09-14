using Application.Interfaces;
using Domain.Common;

namespace Application.Readers.Services
{
    public class ReaderService : IReaderService
    {
        private readonly IReaderRepository _readerRepository;

        public ReaderService(IReaderRepository readerRepository)
        {
            _readerRepository = readerRepository;
        }

        public async Task<Domain.Models.Reader> EnsureExistsAsync(string readerId, CancellationToken ct = default)
        {
            var reader = await _readerRepository.GetByIdAsync(readerId, ct);
            if (reader == null) throw new DomainException("Reader not found.");
            return reader;
        }
    }
}
