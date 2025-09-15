namespace GetReaderById
{
    public interface IGetReaderByIdQueryHandler
    {
        Task<GetReaderByIdQueryOutput?> Handle(GetReaderByIdQueryInput query, CancellationToken ct = default);
    }
}
