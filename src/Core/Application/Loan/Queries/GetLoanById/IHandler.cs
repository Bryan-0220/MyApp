using Domain.Results;

namespace GetLoanById
{
    public interface IGetLoanByIdQueryHandler
    {
        Task<Result<GetLoanByIdQueryOutput>> Handle(GetLoanByIdQueryInput query, CancellationToken ct);
    }
}
