namespace GetAuthorById
{
    public interface IGetAuthorByIdQueryHandler
    {
        Task<GetAuthorByIdQueryOutput?> Handle(GetAuthorByIdQueryInput query, CancellationToken ct = default);
    }
}
