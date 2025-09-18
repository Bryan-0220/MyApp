using Domain.Results;

namespace GetBookById
{
    public interface IGetBookByIdQueryHandler
    {
        Task<Result<GetBookByIdQueryOutput>> Handle(GetBookByIdQueryInput query, CancellationToken ct);
    }
}
