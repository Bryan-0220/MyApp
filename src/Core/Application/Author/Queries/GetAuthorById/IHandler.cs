namespace GetAuthorById
{
    public interface IGetAuthorByIdQueryHandler
    {
        Task<GetAuthorByIdQueryOutput?> HandleAsync(GetAuthorByIdQueryInput query, CancellationToken ct = default);
    }
}
