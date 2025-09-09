namespace GetReaderById
{
    public interface IGetReaderByIdQueryHandler
    {
        Task<GetReaderByIdQueryOutput?> HandleAsync(GetReaderByIdQueryInput query, CancellationToken ct = default);
    }
}
