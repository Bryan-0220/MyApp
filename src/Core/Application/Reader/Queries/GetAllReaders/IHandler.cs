namespace GetAllReaders
{
    public interface IGetAllReadersQueryHandler
    {
        Task<IEnumerable<GetAllReadersQueryOutput>> HandleAsync(GetAllReadersQueryInput query, CancellationToken ct = default);
    }
}
