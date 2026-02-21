namespace GetAllReaders
{
    public interface IGetAllReadersQueryHandler
    {
        Task<IEnumerable<GetAllReadersQueryOutput>> Handle(GetAllReadersQueryInput query, CancellationToken ct = default);
    }
}
