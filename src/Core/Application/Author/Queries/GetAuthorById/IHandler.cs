using Domain.Results;

namespace GetAuthorById
{
    public interface IGetAuthorByIdQueryHandler
    {
        Task<Result<GetAuthorByIdQueryOutput>> Handle(GetAuthorByIdQueryInput query, CancellationToken ct = default);
    }
}
