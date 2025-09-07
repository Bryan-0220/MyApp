namespace GetBookById
{
    public interface IGetBookByIdQueryHandler
    {
        Task<GetBookByIdQueryOutput?> HandleAsync(GetBookByIdQueryInput query, CancellationToken ct);
    }
}
