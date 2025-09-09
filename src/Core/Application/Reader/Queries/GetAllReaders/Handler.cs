using Application.Interfaces;

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

            var projected = users.Select(r => new GetAllReadersQueryOutput
            {
                Id = r.Id,
                FirstName = r.FirstName,
                LastName = r.LastName,
                Email = r.Email,
                MembershipDate = r.MembershipDate
            });

            return projected;
        }
    }
}
