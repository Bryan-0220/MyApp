using Domain.Results;

namespace GetReaderById
{
    public interface IGetReaderByIdQueryHandler
    {
        Task<Result<GetReaderByIdQueryOutput>> Handle(GetReaderByIdQueryInput query, CancellationToken ct = default);
    }
}
